using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CustomerHub.DAL.Models;

public partial class ContactDetail
{
    [Key]
    public int ContactId { get; set; }

    public int CustomerId { get; set; }

    [StringLength(128)]
    public string FullName { get; set; } = null!;

    [StringLength(128)]
    public string? UserName { get; set; }

    [StringLength(128)]
    public string? TelePhone { get; set; }

    [StringLength(128)]
    public string? Email { get; set; }

    [StringLength(128)]
    public string? MailingList { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("ContactDetails")]
    public virtual CustomerDetail Customer { get; set; } = null!;
}
