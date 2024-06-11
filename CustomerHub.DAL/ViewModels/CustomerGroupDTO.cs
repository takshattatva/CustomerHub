using CustomerHub.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerHub.DAL.ViewModels
{
    public class CustomerGroupDTO
    {
        public int CallId { get; set; }
        public int CustomerId { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string SupplierGroupName { get; set; }  = string.Empty;

        public List<SupplierGroup> SupplierGroups { get; set; } = new List<SupplierGroup>();

        public int SupplierGroupId { get; set; }
    }
}