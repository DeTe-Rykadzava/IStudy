using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IStudyIdentityServer.Data;

public partial class IStudyDataBaseContext : DbContext
{
    public IStudyDataBaseContext()
    {
    }

    public IStudyDataBaseContext(DbContextOptions<IStudyDataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder, IConfiguration appConfig)
        => optionsBuilder.UseNpgsql(appConfig.GetConnectionString("DefaultConnect"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            entity.Property(e => e.VerifyStage).HasColumnName("verify_stage");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
