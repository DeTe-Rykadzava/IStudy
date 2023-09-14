using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IStudyIndentityServer.Data;

public partial class IStudyContext : DbContext
{
    public IStudyContext()
    {
    }

    public IStudyContext(DbContextOptions<IStudyContext> options)
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
            entity.HasKey(e => e.Id).HasName("student_pk");

            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Classid).HasColumnName("classid");
            entity.Property(e => e.Firstname).HasColumnName("firstname");
            entity.Property(e => e.Lastname).HasColumnName("lastname");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Secondname).HasColumnName("secondname");
            entity.Property(e => e.Usertypeid).HasColumnName("usertypeid");
            entity.Property(e => e.Verifystage).HasColumnName("verifystage");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
