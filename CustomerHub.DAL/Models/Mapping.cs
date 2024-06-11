using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CustomerHub.DAL.Models;

[Table("Mapping")]
public partial class Mapping
{
    [Key]
    public int MappingId { get; set; }

    public int CustomerId { get; set; }

    public int SupplierGroupId { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Mappings")]
    public virtual CustomerDetail Customer { get; set; } = null!;

    [ForeignKey("SupplierGroupId")]
    [InverseProperty("Mappings")]
    public virtual SupplierGroup SupplierGroup { get; set; } = null!;
}
