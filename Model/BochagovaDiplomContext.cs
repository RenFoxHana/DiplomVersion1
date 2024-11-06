using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DiplomVersion1.Model;

public partial class BochagovaDiplomContext : DbContext
{
    public BochagovaDiplomContext()
    {
    }

    public BochagovaDiplomContext(DbContextOptions<BochagovaDiplomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Institute> Institutes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=NATBOK\\MSSQLSERVER2;Initial Catalog=Bochagova_Diplom;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepartment).HasName("PK__Departme__E1319564F686B971");

            entity.ToTable("Department");

            entity.Property(e => e.IdDepartment).HasColumnName("ID_Department");
            entity.Property(e => e.IdInstitute).HasColumnName("ID_Institute");
            entity.Property(e => e.NameDep)
                .HasMaxLength(70)
                .IsUnicode(false);

            entity.HasOne(d => d.IdInstituteNavigation).WithMany(p => p.Departments)
                .HasForeignKey(d => d.IdInstitute)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Departmen__ID_In__403A8C7D");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.IdEmployee).HasName("PK__Employee__D9EE4F367EC7CB40");

            entity.ToTable("Employee");

            entity.Property(e => e.IdEmployee).HasColumnName("ID_Employee");
            entity.Property(e => e.FirstName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("First_Name");
            entity.Property(e => e.IdDepartment).HasColumnName("ID_Department");
            entity.Property(e => e.IdPost).HasColumnName("ID_Post");
            entity.Property(e => e.LastName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Last_Name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.IdDepartment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__ID_Dep__45F365D3");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.IdPost)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__ID_Pos__44FF419A");
        });

        modelBuilder.Entity<Institute>(entity =>
        {
            entity.HasKey(e => e.IdInstitute).HasName("PK__Institut__3F3CE46CED72C320");

            entity.ToTable("Institute");

            entity.Property(e => e.IdInstitute).HasColumnName("ID_Institute");
            entity.Property(e => e.NameIns)
                .HasMaxLength(70)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.IdPost).HasName("PK__Post__B41D0E30CABD96E5");

            entity.ToTable("Post");

            entity.Property(e => e.IdPost).HasColumnName("ID_Post");
            entity.Property(e => e.NamePost)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
