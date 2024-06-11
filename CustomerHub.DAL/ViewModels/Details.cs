using System.ComponentModel.DataAnnotations;

namespace CustomerHub.DAL.ViewModels
{
    public class Details
    {
        public int PageNumber { get; set; }
        [Required]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? Relation { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string AcCode { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        

        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid TelePhone Number")]
        public string? TelePhone { get; set; }

        [StringLength(32, ErrorMessage = "Only 32 Characaters are Accepted")]
        public string? Address1 { get; set; }

        [StringLength(32, ErrorMessage = "Only 32 Characaters are Accepted")]
        public string? Address2 { get; set; }

        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid Country Name")]
        public string? Town { get; set; }

        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid Country Name")]
        public string? Country { get; set; }

        [RegularExpression(@"^\d{5,10}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string? PostalCode { get; set; }

        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? Currency { get; set; }

        public string? IsSubscribed { get; set; }

        public int AcId { get; set; }

        public string? SupplierList { get; set; }
    }
}