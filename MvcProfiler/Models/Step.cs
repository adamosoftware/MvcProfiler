using System.ComponentModel.DataAnnotations;

namespace MvcProfiler.Library.Models
{
	/// <summary>
	/// A point in time marked within a Request
	/// </summary>
	public class Step
	{
		public long Id { get; set; }

		public int Number { get; set; }

		public long RequestId { get; set; }

		[Required]
		public string Message { get; set; }

		/// <summary>
		/// Milliseconds elapsed to this point
		/// </summary>
		public long Elapsed { get; set; }
	}
}