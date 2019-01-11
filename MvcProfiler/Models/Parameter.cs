using System.ComponentModel.DataAnnotations;

namespace MvcProfiler.Library.Models
{
	/// <summary>
	/// A query string parameter associated with a Request
	/// </summary>
	public class Parameter
	{
		public long Id { get; set; }

		public long RequestId { get; set; }

		[MaxLength(255)]
		[Required]
		public string Name { get; set; }

		[Required]
		public string Value { get; set; }
	}
}