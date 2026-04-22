using EMS.API.Controllers;
using EMS.API.DTOs;
using EMS.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace EMS.Tests.Controllers
{
    [TestFixture]
    public class EmployeesControllerTests
    {
        private Mock<EmployeeService> _serviceMock = null!;
        private EmployeesController  _controller   = null!;

        [SetUp]
        public void Setup()
        {
            var repoMock = new Mock<IEmployeeRepository>();
            _serviceMock = new Mock<EmployeeService>(repoMock.Object);
            _controller  = new EmployeesController(_serviceMock.Object);
        }

        // ── GET /api/employees ────────────────────────────────────────────────

        [Test]
        public async Task GetAll_ReturnsOk_WithPagedResult()
        {
            // Arrange
            var pagedResult = new PagedResult<EmployeeResponseDto>
            {
                Data        = new List<EmployeeResponseDto>
                {
                    new() { Id = 1, FirstName = "Priya", LastName = "Prabhu", Department = "Engineering", Status = "Active" },
                    new() { Id = 2, FirstName = "Arjun", LastName = "Sharma", Department = "Marketing",   Status = "Active" }
                },
                TotalCount  = 15,
                Page        = 1,
                PageSize    = 10,
                TotalPages  = 2,
                HasNextPage = true,
                HasPrevPage = false
            };

            var queryParams = new EmployeeQueryParams { Page = 1, PageSize = 10 };
            _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<EmployeeQueryParams>()))
                        .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAll(queryParams);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (PagedResult<EmployeeResponseDto>)ok.Value!;
            Assert.That(data.TotalCount,  Is.EqualTo(15));
            Assert.That(data.Data.Count,  Is.EqualTo(2));
            Assert.That(data.HasNextPage, Is.True);
        }

        [Test]
        public async Task GetAll_WithSearchFilter_ReturnsFilteredResult()
        {
            // Arrange
            var pagedResult = new PagedResult<EmployeeResponseDto>
            {
                Data        = new List<EmployeeResponseDto>
                {
                    new() { Id = 1, FirstName = "Priya", LastName = "Prabhu", Department = "Engineering", Status = "Active" }
                },
                TotalCount  = 1,
                Page        = 1,
                PageSize    = 10,
                TotalPages  = 1,
                HasNextPage = false,
                HasPrevPage = false
            };

            _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<EmployeeQueryParams>()))
                        .ReturnsAsync(pagedResult);

            var queryParams = new EmployeeQueryParams { Search = "priya", Page = 1, PageSize = 10 };

            // Act
            var result = await _controller.GetAll(queryParams);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (PagedResult<EmployeeResponseDto>)ok.Value!;
            Assert.That(data.TotalCount,        Is.EqualTo(1));
            Assert.That(data.Data[0].FirstName, Is.EqualTo("Priya"));
        }

        // ── GET /api/employees/{id} ───────────────────────────────────────────

        [Test]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            // Arrange
            var emp = new EmployeeResponseDto
            {
                Id          = 1,
                FirstName   = "Priya",
                LastName    = "Prabhu",
                Email       = "priya.prabhu@hexacore.com",
                Department  = "Engineering",
                Status      = "Active"
            };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(emp);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (EmployeeResponseDto)ok.Value!;
            Assert.That(data.Id,        Is.EqualTo(1));
            Assert.That(data.FirstName, Is.EqualTo("Priya"));
        }

        [Test]
        public async Task GetById_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(9999))
                        .ReturnsAsync((EmployeeResponseDto?)null);

            // Act
            var result = await _controller.GetById(9999);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        // ── GET /api/employees/dashboard ─────────────────────────────────────

        [Test]
        public async Task GetDashboard_ReturnsOk_WithSummary()
        {
            // Arrange
            var summary = new DashboardSummaryDto
            {
                Total       = 15,
                Active      = 12,
                Inactive    = 3,
                Departments = 5,
                DepartmentBreakdown = new List<DepartmentBreakdownDto>
                {
                    new() { Department = "Engineering", Count = 5, Percentage = 33 }
                },
                RecentEmployees = new List<EmployeeResponseDto>
                {
                    new() { Id = 15, FirstName = "Pooja", LastName = "Ghosh" }
                }
            };

            _serviceMock.Setup(s => s.GetDashboardSummaryAsync()).ReturnsAsync(summary);

            // Act
            var result = await _controller.GetDashboard();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (DashboardSummaryDto)ok.Value!;
            Assert.That(data.Total,       Is.EqualTo(15));
            Assert.That(data.Active,      Is.EqualTo(12));
            Assert.That(data.Inactive,    Is.EqualTo(3));
            Assert.That(data.Departments, Is.EqualTo(5));
            Assert.That(data.DepartmentBreakdown.Count, Is.EqualTo(1));
            Assert.That(data.RecentEmployees.Count,     Is.EqualTo(1));
        }

        // ── POST /api/employees ───────────────────────────────────────────────

        [Test]
        public async Task Create_ValidData_ReturnsCreated()
        {
            // Arrange
            var dto = new EmployeeRequestDto
            {
                FirstName   = "New",
                LastName    = "Employee",
                Email       = "new.emp@hexacore.com",
                Phone       = "9000000001",
                Department  = "HR",
                Designation = "HR Executive",
                Salary      = 500000m,
                JoinDate    = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Status      = "Active"
            };

            var created = new EmployeeResponseDto
            {
                Id          = 16,
                FirstName   = dto.FirstName,
                LastName    = dto.LastName,
                Email       = dto.Email,
                Phone       = dto.Phone,
                Department  = dto.Department,
                Designation = dto.Designation,
                Salary      = dto.Salary,
                JoinDate    = dto.JoinDate,
                Status      = dto.Status
            };

            _serviceMock.Setup(s => s.CreateAsync(dto))
                        .ReturnsAsync((created, (string?)null));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = (CreatedAtActionResult)result;
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            var data = (EmployeeResponseDto)createdResult.Value!;
            Assert.That(data.Id,        Is.EqualTo(16));
            Assert.That(data.FirstName, Is.EqualTo("New"));
        }

        [Test]
        public async Task Create_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var dto = new EmployeeRequestDto
            {
                FirstName   = "Duplicate",
                LastName    = "Email",
                Email       = "priya.prabhu@hexacore.com",
                Phone       = "9000000002",
                Department  = "HR",
                Designation = "HR Executive",
                Salary      = 500000m,
                JoinDate    = DateTime.UtcNow,
                Status      = "Active"
            };

            _serviceMock.Setup(s => s.CreateAsync(dto))
                        .ReturnsAsync(((EmployeeResponseDto?)null, "An employee with this email already exists."));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }

        // ── PUT /api/employees/{id} ───────────────────────────────────────────

        [Test]
        public async Task Update_ValidId_ReturnsOk()
        {
            // Arrange
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

            var updated = new EmployeeResponseDto
            {
                Id          = 1,
                FirstName   = dto.FirstName,
                LastName    = dto.LastName,
                Email       = dto.Email,
                Designation = dto.Designation,
                Salary      = dto.Salary,
                Status      = dto.Status
            };

            _serviceMock.Setup(s => s.UpdateAsync(1, dto))
                        .ReturnsAsync((updated, (string?)null));

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (EmployeeResponseDto)ok.Value!;
            Assert.That(data.Designation, Is.EqualTo("Senior Software Engineer"));
            Assert.That(data.Salary,      Is.EqualTo(950000m));
        }

        [Test]
        public async Task Update_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var dto = new EmployeeRequestDto
            {
                FirstName   = "Ghost",
                LastName    = "User",
                Email       = "ghost@hexacore.com",
                Phone       = "9000000003",
                Department  = "HR",
                Designation = "Ghost",
                Salary      = 100000m,
                JoinDate    = DateTime.UtcNow,
                Status      = "Active"
            };

            _serviceMock.Setup(s => s.UpdateAsync(9999, dto))
                        .ReturnsAsync(((EmployeeResponseDto?)null, "Employee not found."));

            // Act
            var result = await _controller.Update(9999, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Update_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var dto = new EmployeeRequestDto
            {
                FirstName   = "Arjun",
                LastName    = "Sharma",
                Email       = "priya.prabhu@hexacore.com", // belongs to another employee
                Phone       = "9123456780",
                Department  = "Marketing",
                Designation = "Marketing Executive",
                Salary      = 620000m,
                JoinDate    = DateTime.UtcNow,
                Status      = "Active"
            };

            _serviceMock.Setup(s => s.UpdateAsync(2, dto))
                        .ReturnsAsync(((EmployeeResponseDto?)null, "An employee with this email already exists."));

            // Act
            var result = await _controller.Update(2, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }

        // ── DELETE /api/employees/{id} ────────────────────────────────────────

        [Test]
        public async Task Delete_ExistingId_ReturnsOk()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1))
                        .ReturnsAsync((true, (string?)null));

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Delete_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(9999))
                        .ReturnsAsync((false, "Employee not found."));

            // Act
            var result = await _controller.Delete(9999);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}
