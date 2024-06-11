using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CustomerHub.DAL.Models;

public partial class Group
{
    [Key]
    public int GroupId { get; set; }

    [StringLength(128)]
    public string GroupName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<Mapping> Mappings { get; set; } = new List<Mapping>();

    [InverseProperty("Group")]
    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
