using Microsoft.EntityFrameworkCore;

namespace DiplomVersion1.Model;

public partial class BochagovaDiplomContext : DbContext
{
    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Institute> Institutes { get; set; }

    public virtual DbSet<Key> Keys { get; set; }

    public virtual DbSet<LogOfIssuingKey> LogOfIssuingKeys { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Watchman> Watchmen { get; set; }

    public BochagovaDiplomContext()
    {
        Database.EnsureCreated();
    }

    public BochagovaDiplomContext(DbContextOptions<BochagovaDiplomContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=BochagovaDiplom.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.IdAdmin);

            entity.ToTable("Admin");

            entity.Property(e => e.IdAdmin).HasColumnName("ID_Admin");
            entity.Property(e => e.AdminLogin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Admin_Login");
            entity.Property(e => e.AdminPassword)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Admin_Password");
            entity.Property(e => e.FirstNameAdmin)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("FirstName_Admin");
            entity.Property(e => e.LastNameAdmin)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LastName_Admin");
            entity.Property(e => e.PatronymicAdmin)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Patronymic_Admin");
        });

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
                .HasConstraintName("FK__Employee__ID_Dep__45F365D3");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.IdPost)
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

        modelBuilder.Entity<Key>(entity =>
        {
            entity.HasKey(e => e.IdKey);

            entity.ToTable("Key");

            entity.Property(e => e.IdKey).HasColumnName("ID_Key");
            entity.Property(e => e.AudienceNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Audience_Number");
            entity.Property(e => e.IdDepartment).HasColumnName("ID_Department");

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Keys)
                .HasForeignKey(d => d.IdDepartment)
                .HasConstraintName("FK_Key_Department");

            entity.Property(e => e.IdInstitute).HasColumnName("ID_Institute");

            entity.HasOne(d => d.IdInstituteNavigation).WithMany(p => p.Keys)
                .HasForeignKey(d => d.IdInstitute)
                .HasConstraintName("FK_Key_Institute");
        });

        modelBuilder.Entity<LogOfIssuingKey>(entity =>
        {
            entity.HasKey(e => e.IdEntry);

            entity.ToTable("Log_Of_Issuing_Keys");

            entity.Property(e => e.IdEntry).HasColumnName("ID_Entry");
            entity.Property(e => e.DateTimeOfDelivery)
                .HasColumnType("datetime")
                .HasColumnName("Date_Time_Of_Delivery");
            entity.Property(e => e.DateTimeOfIssue)
                .HasColumnType("datetime")
                .HasColumnName("Date_Time_Of_Issue");
            entity.Property(e => e.IdEmployee).HasColumnName("ID_Employee");
            entity.Property(e => e.IdKey).HasColumnName("ID_Key");
            entity.Property(e => e.IdWatchman).HasColumnName("ID_Watchman");

            entity.HasOne(d => d.IdEmployeeNavigation).WithMany(p => p.LogOfIssuingKeys)
                .HasForeignKey(d => d.IdEmployee)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Log_Of_Issuing_Keys_Employee");

            entity.HasOne(d => d.IdKeyNavigation).WithMany(p => p.LogOfIssuingKeys)
                .HasForeignKey(d => d.IdKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Log_Of_Issuing_Keys_Key");

            entity.HasOne(d => d.IdWatchmanNavigation).WithMany(p => p.LogOfIssuingKeys)
                .HasForeignKey(d => d.IdWatchman)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Log_Of_Issuing_Keys_Watchman");
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

        modelBuilder.Entity<Watchman>(entity =>
        {
            entity.HasKey(e => e.IdWatchman);

            entity.ToTable("Watchman");

            entity.Property(e => e.IdWatchman).HasColumnName("ID_Watchman");
            entity.Property(e => e.FirstNameWm)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("FirstName_WM");
            entity.Property(e => e.LastNameWm)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LastName_WM");
            entity.Property(e => e.PatronymicWm)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Patronymic_WM");
            entity.Property(e => e.WmLogin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WM_Login");
            entity.Property(e => e.WmPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WM_Password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
