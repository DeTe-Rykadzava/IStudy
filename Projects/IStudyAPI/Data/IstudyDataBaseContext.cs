using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IStudyAPI.Data;

public partial class IstudyDataBaseContext : DbContext
{
    public IstudyDataBaseContext()
    {
    }

    public IstudyDataBaseContext(DbContextOptions<IstudyDataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Certificate> Certificates { get; set; }

    public virtual DbSet<CertificateOwner> CertificateOwners { get; set; }

    public virtual DbSet<CertificateOwnerType> CertificateOwnerTypes { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=IStudyDataBase;Username=postgres;Password=root;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("certificate_pk");

            entity.ToTable("certificate");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddedUserId).HasColumnName("added_user_id");
            entity.Property(e => e.Createdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdate");
            entity.Property(e => e.FilePath).HasColumnName("file_path");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.AddedUser).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.AddedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("certificate_added_user_fk");
        });

        modelBuilder.Entity<CertificateOwner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("certificate_owner_pk");

            entity.ToTable("certificate_owner");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CertificateId).HasColumnName("certificate_id");
            entity.Property(e => e.CertificateOwnerType).HasColumnName("certificate_owner_type");
            entity.Property(e => e.CertificateOwnerUserId).HasColumnName("certificate_owner_user_id");

            entity.HasOne(d => d.Certificate).WithMany(p => p.CertificateOwners)
                .HasForeignKey(d => d.CertificateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("certificate_owner_certificate_fk");

            entity.HasOne(d => d.CertificateOwnerTypeNavigation).WithMany(p => p.CertificateOwners)
                .HasForeignKey(d => d.CertificateOwnerType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("certificate_owner_owner_type_fk");

            entity.HasOne(d => d.CertificateOwnerUser).WithMany(p => p.CertificateOwners)
                .HasForeignKey(d => d.CertificateOwnerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("certificate_owner_owner_user_fk");
        });

        modelBuilder.Entity<CertificateOwnerType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("certificate_owner_type_pk");

            entity.ToTable("certificate_owner_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OwnerType).HasColumnName("owner_type");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("class_pk");

            entity.ToTable("class");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pk");

            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.Firstname).HasColumnName("firstname");
            entity.Property(e => e.Lastname).HasColumnName("lastname");
            entity.Property(e => e.Secondname).HasColumnName("secondname");
            entity.Property(e => e.UserPhoto).HasColumnName("user_photo");
            entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");
            entity.Property(e => e.Verifystage).HasColumnName("verifystage");

            entity.HasOne(d => d.Class).WithMany(p => p.Users)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("user_class_fk");

            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_user_type_fk");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_type_pk");

            entity.ToTable("user_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type).HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
