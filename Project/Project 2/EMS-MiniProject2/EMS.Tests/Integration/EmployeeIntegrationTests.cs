using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EMS.Tests.Integration
{
    [TestFixture]
    public class EmployeeIntegrationTests
    {
        private AppDbContext        _db   = null!;
        private EmployeeRepository  _repo = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _db   = new AppDbContext(options);
            _repo = new EmployeeRepository(_db);

            // Seed 3 employees
            _db.Employees.AddRange(
                new Employee { Id = 1, FirstName = "Priya",  LastName = "Prabhu", Email = "priya@test.com",  Phone = "9876543210", Department = "Engineering", Designation = "Dev",    Salary = 800000m, JoinDate = new DateTime(2021,1,1,0,0,0,DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2021,1,1,0,0,0,DateTimeKind.Utc), UpdatedAt = DateTime.UtcNow },
                new Employee { Id = 2, FirstName = "Arjun",  LastName = "Sharma", Email = "arjun@test.com",  Phone = "9123456780", Department = "Marketing",   Designation = "Exec",   Salary = 600000m, JoinDate = new DateTime(2020,6,1,0,0,0,DateTimeKind.Utc), Status = "Active",   CreatedAt = new DateTime(2020,6,1,0,0,0,DateTimeKind.Utc), UpdatedAt = DateTime.UtcNow },
                new Employee { Id = 3, FirstName = "Divya",  LastName = "Nair",   Email = "divya@test.com",  Phone = "9754123456", Department = "HR",          Designation = "Talent", Salary = 530000m, JoinDate = new DateTime(2021,8,1,0,0,0,DateTimeKind.Utc), Status = "Inactive", CreatedAt = new DateTime(2021,8,1,0,0,0,DateTimeKind.Utc), UpdatedAt = DateTime.UtcNow }
            );
            _db.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        // ── Add + Retrieve ────────────────────────────────────────────────────

        [Test]
        public async Task AddAsync_Persists_AndIsRetrievable()
        {
            // Arrange
            var emp = new Employee
            {
                FirstName   = "NewEmp",
                LastName    = "Test",
                Email       = "newemp@test.com",
                Phone       = "9000000001",
                Department  = "Finance",
                Designation = "Analyst",
                Salary      = 700000m,
                JoinDate    = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Status      = "Active"
            };

            // Act
            var created  = await _repo.AddAsync(emp);
            var retrieved = await _repo.GetByIdAsync(created.Id);

            // Assert
            Assert.That(retrieved,           Is.Not.Null);
            Assert.That(retrieved!.FirstName, Is.EqualTo("NewEmp"));
            Assert.That(retrieved.Email,      Is.EqualTo("newemp@test.com"));
        }

        // ── Delete removes record ─────────────────────────────────────────────

        [Test]
        public async Task DeleteAsync_RemovesRecord()
        {
            // Act
            var deleted = await _repo.DeleteAsync(1);
            var result  = await _repo.GetByIdAsync(1);

            // Assert
            Assert.That(deleted, Is.True);
            Assert.That(result,  Is.Null);
        }

        [Test]
        public async Task DeleteAsync_NonExistentId_ReturnsFalse()
        {
            var deleted = await _repo.DeleteAsync(9999);
            Assert.That(deleted, Is.False);
        }

        // ── Email uniqueness ──────────────────────────────────────────────────

        [Test]
        public async Task EmailExistsAsync_ExistingEmail_ReturnsTrue()
        {
            var exists = await _repo.EmailExistsAsync("priya@test.com");
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task EmailExistsAsync_NewEmail_ReturnsFalse()
        {
            var exists = await _repo.EmailExistsAsync("brand.new@test.com");
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task EmailExistsAsync_ExcludesSameEmployee_ReturnsFalse()
        {
            // Editing employee 1 and keeping same email — should NOT be a conflict
            var exists = await _repo.EmailExistsAsync("priya@test.com", excludeId: 1);
            Assert.That(exists, Is.False);
        }

        // ── Dashboard summary counts ──────────────────────────────────────────

        [Test]
        public async Task GetDashboardSummaryAsync_ReturnsCorrectCounts()
        {
            var summary = await _repo.GetDashboardSummaryAsync();

            Assert.That(summary.Total,       Is.EqualTo(3));
            Assert.That(summary.Active,      Is.EqualTo(2));
            Assert.That(summary.Inactive,    Is.EqualTo(1));
            Assert.That(summary.Departments, Is.EqualTo(3)); // Engineering, Marketing, HR
        }

        [Test]
        public async Task GetDashboardSummaryAsync_ReturnsCorrectDepartmentBreakdown()
        {
            var summary = await _repo.GetDashboardSummaryAsync();

            Assert.That(summary.DepartmentBreakdown, Is.Not.Empty);
            var engDept = summary.DepartmentBreakdown.FirstOrDefault(d => d.Department == "Engineering");
            Assert.That(engDept,       Is.Not.Null);
            Assert.That(engDept!.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetDashboardSummaryAsync_ReturnsUpTo5RecentEmployees()
        {
            var summary = await _repo.GetDashboardSummaryAsync();
            Assert.That(summary.RecentEmployees.Count, Is.LessThanOrEqualTo(5));
        }

        // ── Pagination ────────────────────────────────────────────────────────

        [Test]
        public async Task GetAllAsync_Pagination_ReturnsCorrectPage()
        {
            var queryParams = new EmployeeQueryParams { Page = 1, PageSize = 2, SortBy = "name", SortDir = "asc" };

            var result = await _repo.GetAllAsync(queryParams);

            Assert.That(result.TotalCount,  Is.EqualTo(3));
            Assert.That(result.Data.Count,  Is.EqualTo(2));
            Assert.That(result.TotalPages,  Is.EqualTo(2));
            Assert.That(result.HasNextPage, Is.True);
            Assert.That(result.HasPrevPage, Is.False);
        }

        [Test]
        public async Task GetAllAsync_StatusFilter_ReturnsOnlyActive()
        {
            var queryParams = new EmployeeQueryParams { Status = "Active", Page = 1, PageSize = 10 };

            var result = await _repo.GetAllAsync(queryParams);

            Assert.That(result.TotalCount, Is.EqualTo(2));
            Assert.That(result.Data.All(e => e.Status == "Active"), Is.True);
        }

        [Test]
        public async Task GetAllAsync_SearchByName_ReturnsMatchingEmployee()
        {
            var queryParams = new EmployeeQueryParams { Search = "priya", Page = 1, PageSize = 10 };

            var result = await _repo.GetAllAsync(queryParams);

            Assert.That(result.TotalCount,           Is.EqualTo(1));
            Assert.That(result.Data[0].FirstName,    Is.EqualTo("Priya"));
        }
    }
}
