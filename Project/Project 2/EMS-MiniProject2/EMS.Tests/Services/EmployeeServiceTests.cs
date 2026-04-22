using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Moq;
using NUnit.Framework;

namespace EMS.Tests.Services
{
    [TestFixture]
    public class EmployeeServiceTests
    {
        private Mock<IEmployeeRepository> _repoMock = null!;
        private EmployeeService           _service  = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IEmployeeRepository>();
            _service  = new EmployeeService(_repoMock.Object);
        }

        // ── GetByIdAsync ──────────────────────────────────────────────────────

        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsMappedDto()
        {
            // Arrange
            var fakeEmployee = new Employee
            {
                Id          = 1,
                FirstName   = "Priya",
                LastName    = "Prabhu",
                Email       = "priya.prabhu@hexacore.com",
                Phone       = "9876543210",
                Department  = "Engineering",
                Designation = "Software Engineer",
                Salary      = 850000m,
                JoinDate    = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Status      = "Active",
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeEmployee);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.That(result,              Is.Not.Null);
            Assert.That(result!.Id,          Is.EqualTo(1));
            Assert.That(result.FirstName,    Is.EqualTo("Priya"));
            Assert.That(result.LastName,     Is.EqualTo("Prabhu"));
            Assert.That(result.Email,        Is.EqualTo("priya.prabhu@hexacore.com"));
            Assert.That(result.Department,   Is.EqualTo("Engineering"));
            Assert.That(result.Status,       Is.EqualTo("Active"));
            _repoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(9999)).ReturnsAsync((Employee?)null);

            // Act
            var result = await _service.GetByIdAsync(9999);

            // Assert
            Assert.That(result, Is.Null);
            _repoMock.Verify(r => r.GetByIdAsync(9999), Times.Once);
        }

        // ── CreateAsync ───────────────────────────────────────────────────────

        [Test]
        public async Task CreateAsync_NewEmail_CallsAddAsyncAndReturnsDto()
        {
            // Arrange
            var dto = new EmployeeRequestDto
            {
                FirstName   = "Arjun",
                LastName    = "Sharma",
                Email       = "arjun.sharma@hexacore.com",
                Phone       = "9123456780",
                Department  = "Marketing",
                Designation = "Marketing Executive",
                Salary      = 620000m,
                JoinDate    = new DateTime(2020, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                Status      = "Active"
            };

            _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, null)).ReturnsAsync(false);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                     .ReturnsAsync((Employee e) =>
                     {
                         e.Id = 2;
                         return e;
                     });

            // Act
            var (result, error) = await _service.CreateAsync(dto);

            // Assert
            Assert.That(error,             Is.Null);
            Assert.That(result,            Is.Not.Null);
            Assert.That(result!.FirstName, Is.EqualTo("Arjun"));
            Assert.That(result.Email,      Is.EqualTo("arjun.sharma@hexacore.com"));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public async Task CreateAsync_DuplicateEmail_ReturnsConflictError()
        {
            // Arrange
            var dto = new EmployeeRequestDto
            {
                FirstName   = "Test",
                LastName    = "User",
                Email       = "priya.prabhu@hexacore.com",
                Phone       = "9876543210",
                Department  = "Engineering",
                Designation = "Tester",
                Salary      = 500000m,
                JoinDate    = DateTime.UtcNow,
                Status      = "Active"
            };

            _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, null)).ReturnsAsync(true);

            // Act
            var (result, error) = await _service.CreateAsync(dto);

            // Assert
            Assert.That(result, Is.Null);
            Assert.That(error,  Is.Not.Null.And.Contains("already exists"));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
        }

        // ── UpdateAsync ───────────────────────────────────────────────────────

        [Test]
        public async Task UpdateAsync_ValidId_UpdatesEmployee()
        {
            // Arrange
            var existing = new Employee
            {
                Id          = 1,
                FirstName   = "Priya",
                LastName    = "Prabhu",
                Email       = "priya.prabhu@hexacore.com",
                Phone       = "9876543210",
                Department  = "Engineering",
                Designation = "Software Engineer",
                Salary      = 850000m,
                JoinDate    = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Status      = "Active",
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow
            };

            var dto = new EmployeeRequestDto
            {
                FirstName   = "Priya",
                LastName    = "Prabhu",
                Email       = "priya.prabhu@hexacore.com",
                Phone       = "9876543210",
                Department  = "Engineering",
                Designation = "Senior Software Engineer",
                Salary      = 950000m,
                JoinDate    = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Status      = "Active"
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, 1)).ReturnsAsync(false);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
                     .ReturnsAsync((Employee e) => e);

            // Act
            var (result, error) = await _service.UpdateAsync(1, dto);

            // Assert
            Assert.That(error,               Is.Null);
            Assert.That(result,              Is.Not.Null);
            Assert.That(result!.Designation, Is.EqualTo("Senior Software Engineer"));
            Assert.That(result.Salary,       Is.EqualTo(950000m));
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_NonExistentId_ReturnsNotFoundError()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            var dto = new EmployeeRequestDto
            {
                FirstName   = "Ghost",
                LastName    = "User",
                Email       = "ghost@hexacore.com",
                Phone       = "9000000000",
                Department  = "HR",
                Designation = "Ghost",
                Salary      = 100000m,
                JoinDate    = DateTime.UtcNow,
                Status      = "Active"
            };

            // Act
            var (result, error) = await _service.UpdateAsync(999, dto);

            // Assert
            Assert.That(result, Is.Null);
            Assert.That(error,  Is.EqualTo("Employee not found."));
        }

        // ── DeleteAsync ───────────────────────────────────────────────────────

        [Test]
        public async Task DeleteAsync_ValidId_ReturnsSuccess()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var (success, error) = await _service.DeleteAsync(1);

            // Assert
            Assert.That(success, Is.True);
            Assert.That(error,   Is.Null);
            _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_NonExistentId_ReturnsFailure()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(9999)).ReturnsAsync(false);

            // Act
            var (success, error) = await _service.DeleteAsync(9999);

            // Assert
            Assert.That(success, Is.False);
            Assert.That(error,   Is.EqualTo("Employee not found."));
        }

        // ── GetAllAsync ───────────────────────────────────────────────────────

        [Test]
        public async Task GetAllAsync_ReturnsPagedResult()
        {
            // Arrange
            var pagedResult = new PagedResult<EmployeeResponseDto>
            {
                Data       = new List<EmployeeResponseDto> { new() { Id = 1, FirstName = "Priya" } },
                TotalCount = 15,
                Page       = 1,
                PageSize   = 10,
                TotalPages = 2,
                HasNextPage = true,
                HasPrevPage = false
            };

            var queryParams = new EmployeeQueryParams { Page = 1, PageSize = 10 };
            _repoMock.Setup(r => r.GetAllAsync(queryParams)).ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllAsync(queryParams);

            // Assert
            Assert.That(result.TotalCount,  Is.EqualTo(15));
            Assert.That(result.Data.Count,  Is.EqualTo(1));
            Assert.That(result.HasNextPage, Is.True);
        }
    }
}
