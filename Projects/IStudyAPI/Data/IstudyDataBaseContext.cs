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
            entity.Property(e => e.CreateDate).HasColumnName("create_date");
            entity.Property(e => e.Filename).HasColumnName("filename");
            entity.Property(e => e.TeacherUserId).HasColumnName("teacher_user_id");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.AddedUser).WithMany(p => p.CertificateAddedUsers)
                .HasForeignKey(d => d.AddedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("certificate_user_add_fk");

            entity.HasOne(d => d.TeacherUser).WithMany(p => p.CertificateTeacherUsers)
                .HasForeignKey(d => d.TeacherUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("certificate_teacher_fk");
        });

        modelBuilder.Entity<CertificateOwner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("certificate_owner_pk");

            entity.ToTable("certificate_owner");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CertificateId).HasColumnName("certificate_id");
            entity.Property(e => e.UserOwnerId).HasColumnName("user_owner_id");

            entity.HasOne(d => d.Certificate).WithMany(p => p.CertificateOwners)
                .HasForeignKey(d => d.CertificateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("certificate_owner_certificate_fk");

            entity.HasOne(d => d.UserOwner).WithMany(p => p.CertificateOwners)
                .HasForeignKey(d => d.UserOwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("certificate_owner_owner_user_fk");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("class_pk");

            entity.ToTable("class");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClassName).HasColumnName("class_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pk");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.Firstname).HasColumnName("firstname");
            entity.Property(e => e.Lastname).HasColumnName("lastname");
            entity.Property(e => e.Secondname).HasColumnName("secondname");
            entity.Property(e => e.UserPhoto).HasColumnName("user_photo");
            entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Verifystage).HasColumnName("verifystage");

            entity.HasOne(d => d.Class).WithMany(p => p.Users)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_class_fk");

            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_user_type_fk");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usertype_pk");

            entity.ToTable("user_type");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('usertype_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Type).HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
