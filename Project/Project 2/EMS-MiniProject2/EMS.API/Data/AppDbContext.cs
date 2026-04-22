using EMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<AppUser> AppUsers => Set<AppUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique index on Employee.Email
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Unique index on AppUser.Username
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // ── Seed Users ──────────────────────────────────────────────────────
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new AppUser
                {
                    Id = 2,
                    Username = "viewer",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("viewer123"),
                    Role = "Viewer",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // ── Seed Employees (identical to Mini Project 1 data.js) ────────────
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1,  FirstName = "Priya",   LastName = "Prabhu",   Email = "priya.prabhu@hexacore.com",   Phone = "9876543210", Department = "Engineering", Designation = "Software Engineer",   Salary = 850000m,  JoinDate = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 2,  FirstName = "Arjun",   LastName = "Sharma",   Email = "arjun.sharma@hexacore.com",   Phone = "9123456780", Department = "Marketing",   Designation = "Marketing Executive",  Salary = 620000m,  JoinDate = new DateTime(2020, 7, 1,  0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2020, 7, 1,  0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2020, 7, 1,  0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 3,  FirstName = "Neha",    LastName = "Kapoor",   Email = "neha.kapoor@hexacore.com",    Phone = "9988776655", Department = "HR",          Designation = "HR Executive",         Salary = 550000m,  JoinDate = new DateTime(2019, 11, 20, 0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2019, 11, 20, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2019, 11, 20, 0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 4,  FirstName = "Rahul",   LastName = "Verma",    Email = "rahul.verma@hexacore.com",    Phone = "9001234567", Department = "Finance",     Designation = "Financial Analyst",    Salary = 720000m,  JoinDate = new DateTime(2022, 1, 10,  0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2022, 1, 10,  0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2022, 1, 10,  0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 5,  FirstName = "Sneha",   LastName = "Prasad",   Email = "sneha.prasad@hexacore.com",   Phone = "9012345678", Department = "Operations",  Designation = "Operations Manager",   Salary = 950000m,  JoinDate = new DateTime(2018, 6, 5,   0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2018, 6, 5,   0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2018, 6, 5,   0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 6,  FirstName = "Vikram",  LastName = "Raj",      Email = "vikram.raj@hexacore.com",     Phone = "9811223344", Department = "Engineering", Designation = "Senior Developer",     Salary = 1100000m, JoinDate = new DateTime(2017, 9, 12,  0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2017, 9, 12,  0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2017, 9, 12,  0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 7,  FirstName = "Ananya",  LastName = "Singh",    Email = "ananya.singh@hexacore.com",   Phone = "9922334455", Department = "Marketing",   Designation = "Content Strategist",   Salary = 580000m,  JoinDate = new DateTime(2023, 2, 28,  0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = new DateTime(2023, 2, 28,  0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 2, 28,  0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 8,  FirstName = "Karthik", LastName = "Rajan",    Email = "karthik.rajan@hexacore.com",  Phone = "9876001234", Department = "Finance",     Designation = "Accounts Manager",     Salary = 800000m,  JoinDate = new DateTime(2020, 4, 17,  0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2020, 4, 17,  0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2020, 4, 17,  0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 9,  FirstName = "Divya",   LastName = "Nair",     Email = "divya.nair@hexacore.com",     Phone = "9754123456", Department = "HR",          Designation = "Talent Acquisition",   Salary = 530000m,  JoinDate = new DateTime(2021, 8, 9,   0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = new DateTime(2021, 8, 9,   0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2021, 8, 9,   0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 10, FirstName = "Rohan",   LastName = "Mehta",    Email = "rohan.mehta@hexacore.com",    Phone = "9654987321", Department = "Engineering", Designation = "DevOps Engineer",      Salary = 920000m,  JoinDate = new DateTime(2019, 3, 22,  0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2019, 3, 22,  0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2019, 3, 22,  0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 11, FirstName = "Kavya",   LastName = "Iyer",     Email = "kavya.iyer@hexacore.com",     Phone = "9543219876", Department = "Operations",  Designation = "Supply Chain Analyst", Salary = 670000m,  JoinDate = new DateTime(2022, 5, 14,  0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2022, 5, 14,  0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2022, 5, 14,  0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 12, FirstName = "Suresh",  LastName = "Babu",     Email = "suresh.babu@hexacore.com",    Phone = "9432187654", Department = "Finance",     Designation = "Tax Consultant",       Salary = 750000m,  JoinDate = new DateTime(2020, 11, 30, 0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = new DateTime(2020, 11, 30, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2020, 11, 30, 0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 13, FirstName = "Lakshmi", LastName = "Chandran", Email = "lakshmi.c@hexacore.com",      Phone = "9321654321", Department = "Marketing",   Designation = "Brand Manager",        Salary = 880000m,  JoinDate = new DateTime(2018, 12, 1,  0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2018, 12, 1,  0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2018, 12, 1,  0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 14, FirstName = "Amit",    LastName = "Joshi",    Email = "amit.joshi@hexacore.com",     Phone = "9210543219", Department = "Operations",  Designation = "Supply Chain Analyst", Salary = 610000m,  JoinDate = new DateTime(2023, 7, 10,  0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2023, 7, 10,  0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 7, 10,  0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 15, FirstName = "Pooja",   LastName = "Ghosh",    Email = "pooja.ghosh@hexacore.com",    Phone = "9109432108", Department = "Engineering", Designation = "DevOps Engineer",      Salary = 990000m,  JoinDate = new DateTime(2024, 1, 5,   0, 0, 0, DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2024, 1, 5,   0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 5,   0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
