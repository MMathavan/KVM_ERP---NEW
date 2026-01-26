using System.ComponentModel.DataAnnotations;

namespace KVM_ERP.Models
{
	// View model for Category Type create/edit
	public class CategoryMaster
	{
		public int ACID { get; set; }

		[StringLength(15)]
		public string ACCODE { get; set; }

		[Required]
		[StringLength(200)]
		public string ACDESC { get; set; }

		public string CUSRID { get; set; }
		public string LMUSRID { get; set; }

		// "0" = Active, "1" = Inactive
		[Required]
		[RegularExpression("^(0|1)$")]
		public string DISPSTATUS { get; set; }
	}
}
