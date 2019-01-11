using MvcProfiler.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Routing;

namespace MvcProfiler.Library
{
	public abstract class ProfilerBase
	{
		private readonly Stopwatch _stopwatch;

		private int stepNumber = 0;
		private Dictionary<string, TimeSpan> _markers = new Dictionary<string, TimeSpan>();

		private const string defaultMarker = "_default";

		public ProfilerBase(RequestContext requestContext)
		{
			// null can be used on app startup to call VerifyDatabaseObjects
			if (requestContext == null) return;

			_stopwatch = Stopwatch.StartNew();
			_markers.Add(defaultMarker, new TimeSpan(0));

			var route = requestContext.RouteData;
			CurrentRequest = new Request()
			{
				Timestamp = DateTime.UtcNow,
				UserName = requestContext.HttpContext.User.Identity.Name,
				Method = requestContext.HttpContext.Request.HttpMethod,
				Area = route.DataTokens.TryGetValue("area", out object token) ? token as string : string.Empty,
				Controller = route.Values["controller"] as string,
				Action = route.Values["action"] as string,
				Parameters = ParseParameters(requestContext)
			};
		}

		private static IEnumerable<Parameter> ParseParameters(RequestContext requestContext)
		{
			var qs = requestContext.HttpContext.Request.QueryString;
			return qs.AllKeys.Any() ?
				qs.AllKeys.Select(key => new Parameter() { Name = key, Value = qs[key] }) :
				Enumerable.Empty<Parameter>();
		}

		/// <summary>
		/// Use this to create the Request, Step, and Parameter tables in your target database
		/// or wherever you want to persist them
		/// </summary>
		public abstract void VerifyDatabaseObjects();

		/// <summary>
		/// Use this to save the <see cref="CurrentRequest"/> to your target database
		/// or wherever it is you're saving profiler info
		/// </summary>
		protected abstract void Save();

		/// <summary>
		/// Implement this to give Profiler access to your database
		/// </summary>		
		protected abstract IDbConnection GetConnection();

		/// <summary>
		/// Access to request currently being profiled
		/// </summary>
		protected Request CurrentRequest { get; }		

		/// <summary>
		/// Marks the start of a named time segment
		/// </summary>
		/// <param name="name">Name of the step, to be completed in the <see cref="StepEnd"/> method</param>
		public void StepBegin(string name)
		{
			_markers.Add(name, _stopwatch.Elapsed);
		}

		public void StepEnd(string name)
		{
			EndStepInner(name, GetMarker(name));
		}

		private TimeSpan GetMarker(string name)
		{
			try
			{
				return _markers[name];
			}
			catch (Exception exc)
			{
				throw new Exception($"Error getting named Profiler named step: {name}: {exc.Message}");
			}
		}

		/// <summary>
		/// Call this whenever you want to mark the time elapsed during a request at a specific point
		/// relative to the start of the request or the last time you called this method.
		/// </summary>
		public void Step(string message)
		{
			TimeSpan priorStep = _stopwatch.Elapsed;
			EndStepInner(message, _markers[defaultMarker]);
			_markers[defaultMarker] = priorStep;
		}

		private void EndStepInner(string message, TimeSpan startPoint)
		{
			TimeSpan rightNow = _stopwatch.Elapsed;
			var elapsed = rightNow.Subtract(startPoint);
			long ms = (long)elapsed.TotalMilliseconds;

			stepNumber++;
			CurrentRequest.Steps.Add(new Step()
			{
				Message = message,
				Number = stepNumber,
				Elasped = ms
			});
		}

		/// <summary>
		/// Call this in your Dispose override in your base controller
		/// </summary>
		public void Stop()
		{
			try
			{
				_stopwatch.Stop();
				CurrentRequest.Elapsed = (long)_stopwatch.Elapsed.TotalMilliseconds;
				Save();
			}
			catch
			{
				// do nothing, we don't want exceptions escaping into app
			}
		}
	}
}