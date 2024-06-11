using CustomerHub.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerHub.DAL.ViewModels
{
    public class CustomerContactDTO
    {
        public int CallId { get; set; }

        public List<ContactDetail> ContactDetails { get; set; } = new List<ContactDetail>();

        public string? ContactList { get; set; }

        public int CustomerId { get; set; }

        public int ContactId { get; set; }

        [Required]
        [StringLength(32, ErrorMessage = "Only 32 Characaters are Accepted")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? UserName { get; set; }

        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid TelePhone Number")]
        public string? Telephone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        public string? MailingList { get; set; }
    }
}