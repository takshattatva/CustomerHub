using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CustomerHub.DAL.Models;

public partial class CustomerDbContext : DbContext
{
    public CustomerDbContext()
    {
    }

    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ContactDetail> ContactDetails { get; set; }

    public virtual DbSet<CustomerDetail> CustomerDetails { get; set; }

    public virtual DbSet<Mapping> Mappings { get; set; }

    public virtual DbSet<SupplierGroup> SupplierGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("User ID = postgres;Password=!@12Taksh;Server=localhost;Port=5432;Database=customer_db;Integrated Security=true;Pooling=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContactDetail>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("ContactDetails_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("false");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Customer).WithMany(p => p.ContactDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("withCustomer");
        });

        modelBuilder.Entity<CustomerDetail>(entity =>
        {
            entity.HasKey(e => e.AcId).HasName("CustomerDetails_pkey");

            entity.Property(e => e.AcId).UseIdentityAlwaysColumn();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("false");
            entity.Property(e => e.IsSubscribed).HasDefaultValueSql("false");
            entity.Property(e => e.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Mapping>(entity =>
        {
            entity.HasKey(e => e.MappingId).HasName("Mapping_pkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Mappings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("withCustomer");

            entity.HasOne(d => d.SupplierGroup).WithMany(p => p.Mappings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("withGroup");
        });

        modelBuilder.Entity<SupplierGroup>(entity =>
        {
            entity.HasKey(e => e.SupplierGroupId).HasName("Groups_pkey");

            entity.Property(e => e.SupplierGroupId).HasDefaultValueSql("nextval('\"Groups_GroupId_seq\"'::regclass)");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsAssigned).HasDefaultValueSql("false");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("false");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Customer).WithMany(p => p.SupplierGroups).HasConstraintName("withCustomer");
        });
        modelBuilder.HasSequence("CustomerDetails_AcId_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
