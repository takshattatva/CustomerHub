using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CustomerHub.DAL.Models;

public partial class SupplierGroup
{
    [Key]
    public int SupplierGroupId { get; set; }

    [StringLength(128)]
    public string SupplierName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CustomerId { get; set; }

    public bool? IsAssigned { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("SupplierGroups")]
    public virtual CustomerDetail? Customer { get; set; }

    [InverseProperty("SupplierGroup")]
    public virtual ICollection<Mapping> Mappings { get; set; } = new List<Mapping>();
}
