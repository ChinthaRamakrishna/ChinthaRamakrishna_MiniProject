using System.ComponentModel.DataAnnotations;

namespace EMS.API.DTOs
{
    // ── Employee Request DTO (create / update) ──────────────────────────────────
    public class EmployeeRequestDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a department.")]
        [RegularExpression("^(Engineering|Marketing|HR|Finance|Operations)$",
            ErrorMessage = "Department must be one of: Engineering, Marketing, HR, Finance, Operations.")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required.")]
        [MaxLength(100)]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salary is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Join date is required.")]
        public DateTime JoinDate { get; set; }

        [Required(ErrorMessage = "Please select a status.")]
        [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be Active or Inactive.")]
        public string Status { get; set; } = string.Empty;
    }

    // ── Employee Response DTO ───────────────────────────────────────────────────
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // ── Paged Result Envelope ───────────────────────────────────────────────────
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPrevPage { get; set; }
    }

    // ── Query Parameters ────────────────────────────────────────────────────────
    public class EmployeeQueryParams
    {
        public string? Search { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
        public string SortBy { get; set; } = "name";
        public string SortDir { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ── Dashboard DTO ───────────────────────────────────────────────────────────
    public class DashboardSummaryDto
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
        public int Departments { get; set; }
        public List<DepartmentBreakdownDto> DepartmentBreakdown { get; set; } = new();
        public List<EmployeeResponseDto> RecentEmployees { get; set; } = new();
    }

    public class DepartmentBreakdownDto
    {
        public string Department { get; set; } = string.Empty;
        public int Count { get; set; }
        public int Percentage { get; set; }
    }

    // ── Auth DTOs ───────────────────────────────────────────────────────────────
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [RegularExpression("^(Admin|Viewer)$", ErrorMessage = "Role must be Admin or Viewer.")]
        public string Role { get; set; } = "Viewer";
    }

    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
    }
}
