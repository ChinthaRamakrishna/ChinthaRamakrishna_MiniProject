using EMS.API.DTOs;
using EMS.API.Models;

namespace EMS.API.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeService(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        public virtual Task<PagedResult<EmployeeResponseDto>> GetAllAsync(EmployeeQueryParams queryParams)
            => _repo.GetAllAsync(queryParams);

        public virtual async Task<EmployeeResponseDto?> GetByIdAsync(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            return emp == null ? null : MapToDto(emp);
        }

        public virtual async Task<(EmployeeResponseDto? Dto, string? Error)> CreateAsync(EmployeeRequestDto dto)
        {
            if (await _repo.EmailExistsAsync(dto.Email))
                return (null, "An employee with this email already exists.");

            var employee = MapFromDto(dto);
            var created  = await _repo.AddAsync(employee);
            return (MapToDto(created), null);
        }

        public virtual async Task<(EmployeeResponseDto? Dto, string? Error)> UpdateAsync(int id, EmployeeRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return (null, "Employee not found.");

            if (await _repo.EmailExistsAsync(dto.Email, id))
                return (null, "An employee with this email already exists.");

            existing.FirstName   = dto.FirstName;
            existing.LastName    = dto.LastName;
            existing.Email       = dto.Email;
            existing.Phone       = dto.Phone;
            existing.Department  = dto.Department;
            existing.Designation = dto.Designation;
            existing.Salary      = dto.Salary;
            existing.JoinDate    = dto.JoinDate;
            existing.Status      = dto.Status;

            var updated = await _repo.UpdateAsync(existing);
            return (MapToDto(updated), null);
        }

        public virtual async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Employee not found.");
        }

        public virtual Task<DashboardSummaryDto> GetDashboardSummaryAsync()
            => _repo.GetDashboardSummaryAsync();

        // ── Mappers ───────────────────────────────────────────────────────────
        private static EmployeeResponseDto MapToDto(Employee e) => new()
        {
            Id          = e.Id,
            FirstName   = e.FirstName,
            LastName    = e.LastName,
            Email       = e.Email,
            Phone       = e.Phone,
            Department  = e.Department,
            Designation = e.Designation,
            Salary      = e.Salary,
            JoinDate    = e.JoinDate,
            Status      = e.Status,
            CreatedAt   = e.CreatedAt,
            UpdatedAt   = e.UpdatedAt
        };

        private static Employee MapFromDto(EmployeeRequestDto dto) => new()
        {
            FirstName   = dto.FirstName,
            LastName    = dto.LastName,
            Email       = dto.Email,
            Phone       = dto.Phone,
            Department  = dto.Department,
            Designation = dto.Designation,
            Salary      = dto.Salary,
            JoinDate    = dto.JoinDate.ToUniversalTime(),
            Status      = dto.Status
        };
    }
}
