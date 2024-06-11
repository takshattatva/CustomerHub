using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CustomerHub.DAL.Models;

public partial class Supplier
{
    [Key]
    public int SupplierId { get; set; }

    [StringLength(128)]
    public string SupplierName { get; set; } = null!;

    public int? GroupId { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("Suppliers")]
    public virtual Group? Group { get; set; }
}
