using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcProfiler.Library.Models
{
	public class Request
	{
		public long Id { get; set; }

		[MaxLength(50)]
		public string UserName { get; set; }

		[MaxLength(10)]
		[Required]
		public string Method { get; set; }

		[MaxLength(50)]
		[Required]
		public string Area { get; set; }

		[MaxLength(50)]
		[Required]
		public string Controller { get; set; }

		[MaxLength(50)]
		[Required]
		public string Action { get; set; }

		/// <summary>
		/// Start time of request
		/// </summary>
		public DateTime Timestamp { get; set; }

		/// <summary>
		/// Overall milliseconds elapsed
		/// </summary>
		public long Elapsed { get; set; }

		/// <summary>
		/// QueryString parameters parsed from request
		/// </summary>
		public IEnumerable<Parameter> Parameters { get; set; }

		/// <summary>
		/// Steps added dynamically during a request with the Step method
		/// </summary>
		public List<Step> Steps { get; set; } = new List<Step>();
	}
}