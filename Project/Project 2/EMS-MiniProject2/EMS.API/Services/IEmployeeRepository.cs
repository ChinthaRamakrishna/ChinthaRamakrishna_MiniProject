using EMS.API.DTOs;
using EMS.API.Models;

namespace EMS.API.Services
{
    public interface IEmployeeRepository
    {
        Task<PagedResult<EmployeeResponseDto>> GetAllAsync(EmployeeQueryParams queryParams);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByEmailAsync(string email, int? excludeId = null);
        Task<Employee> AddAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    }
}
