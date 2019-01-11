using Dapper;
using MvcProfiler.Library;
using MvcProfiler.Library.Models;
using Postulate.Lite.SqlServer.LongKey;
using System.Data;
using System.Web.Routing;

namespace MvcProfiler.Postulate
{
	public abstract class PostulateProfilerBase : ProfilerBase
	{
		private const string schema = "profiler";
		private const string profilerRequest = schema + ".Request";
		private const string profilerStep = schema + ".Step";
		private const string profilerParam = schema + ".Parameter";

		public PostulateProfilerBase(RequestContext requestContext) : base(requestContext)
		{
		}

		public override void VerifyDatabaseObjects()
		{
			using (var cn = GetConnection())
			{
				if (!SchemaExists(cn, schema))
				{
					cn.Execute($"CREATE SCHEMA [{schema}]");
					cn.CreateTable<Request>(profilerRequest);
					cn.CreateTable<Parameter>(profilerParam);
					cn.CreateTable<Step>(profilerStep);
				}
			}
		}

		private bool SchemaExists(IDbConnection cn, string schema)
		{
			return (cn.QuerySingleOrDefault<int>("SELECT 1 FROM [sys].[schemas] WHERE [name]=@name", new { name = schema }) == 1);
		}

		protected override void Save()
		{
			using (var cn = GetConnection())
			{
				long id = cn.Save(CurrentRequest, tableName: profilerRequest);

				foreach (var p in CurrentRequest.Parameters)
				{
					p.RequestId = id;
					cn.Save(p, tableName: profilerParam);
				}

				foreach (var s in CurrentRequest.Steps)
				{
					s.RequestId = id;
					cn.Save(s, tableName: profilerStep);
				}
			}
		}
	}
}