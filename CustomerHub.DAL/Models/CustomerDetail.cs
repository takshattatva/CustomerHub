using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CustomerHub.DAL.Models;

public partial class CustomerDetail
{
    [Key]
    public int AcId { get; set; }

    [StringLength(128)]
    public string CompanyName { get; set; } = null!;

    [StringLength(100)]
    public string? PostalCode { get; set; }

    [StringLength(23)]
    public string? TelePhone { get; set; }

    [StringLength(100)]
    public string? Relation { get; set; }

    [StringLength(100)]
    public string? Currency { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [StringLength(128)]
    public string? Address1 { get; set; }

    [StringLength(128)]
    public string? Address2 { get; set; }

    [StringLength(100)]
    public string? Town { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    public bool? IsSubscribed { get; set; }

    [StringLength(50)]
    public string AcCode { get; set; } = null!;

    [InverseProperty("Customer")]
    public virtual ICollection<ContactDetail> ContactDetails { get; set; } = new List<ContactDetail>();

    [InverseProperty("Customer")]
    public virtual ICollection<Mapping> Mappings { get; set; } = new List<Mapping>();

    [InverseProperty("Customer")]
    public virtual ICollection<SupplierGroup> SupplierGroups { get; set; } = new List<SupplierGroup>();
}
