using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Services
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _db;

        public EmployeeRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResult<EmployeeResponseDto>> GetAllAsync(EmployeeQueryParams q)
        {
            var query = _db.Employees.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var term = q.Search.ToLower();
                query = query.Where(e =>
                    (e.FirstName + " " + e.LastName).ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term));
            }

            // Department filter
            if (!string.IsNullOrWhiteSpace(q.Department) && q.Department != "All")
                query = query.Where(e => e.Department == q.Department);

            // Status filter
            if (!string.IsNullOrWhiteSpace(q.Status) && q.Status != "All")
                query = query.Where(e => e.Status == q.Status);

            // Sort
            query = q.SortBy?.ToLower() switch
            {
                "salary"   => q.SortDir == "desc" ? query.OrderByDescending(e => e.Salary)   : query.OrderBy(e => e.Salary),
                "joindate" => q.SortDir == "desc" ? query.OrderByDescending(e => e.JoinDate) : query.OrderBy(e => e.JoinDate),
                _          => q.SortDir == "desc"
                                ? query.OrderByDescending(e => e.FirstName).ThenByDescending(e => e.LastName)
                                : query.OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
            };

            var totalCount = await query.CountAsync();

            // Clamp pageSize
            var pageSize = Math.Min(q.PageSize, 100);
            var page     = Math.Max(q.Page, 1);

            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => MapToResponseDto(e))
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedResult<EmployeeResponseDto>
            {
                Data        = employees,
                TotalCount  = totalCount,
                Page        = page,
                PageSize    = pageSize,
                TotalPages  = totalPages,
                HasNextPage = page < totalPages,
                HasPrevPage = page > 1
            };
        }

        public async Task<Employee?> GetByIdAsync(int id)
            => await _db.Employees.FindAsync(id);

        public async Task<Employee?> GetByEmailAsync(string email, int? excludeId = null)
        {
            var query = _db.Employees.Where(e => e.Email.ToLower() == email.ToLower());
            if (excludeId.HasValue)
                query = query.Where(e => e.Id != excludeId.Value);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _db.Employees.Where(e => e.Email.ToLower() == email.ToLower());
            if (excludeId.HasValue)
                query = query.Where(e => e.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            employee.CreatedAt = DateTime.UtcNow;
            employee.UpdatedAt = DateTime.UtcNow;
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            employee.UpdatedAt = DateTime.UtcNow;
            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null) return false;
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var total    = await _db.Employees.CountAsync();
            var active   = await _db.Employees.CountAsync(e => e.Status == "Active");
            var inactive = await _db.Employees.CountAsync(e => e.Status == "Inactive");

            var deptBreakdown = await _db.Employees
                .GroupBy(e => e.Department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .OrderBy(x => x.Department)
                .ToListAsync();

            var breakdown = deptBreakdown.Select(d => new DepartmentBreakdownDto
            {
                Department = d.Department,
                Count      = d.Count,
                Percentage = total > 0 ? (int)Math.Round((d.Count / (double)total) * 100) : 0
            }).ToList();

            var recent = await _db.Employees
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Take(5)
                .Select(e => MapToResponseDto(e))
                .ToListAsync();

            return new DashboardSummaryDto
            {
                Total               = total,
                Active              = active,
                Inactive            = inactive,
                Departments         = deptBreakdown.Count,
                DepartmentBreakdown = breakdown,
                RecentEmployees     = recent
            };
        }

        // ── Static mapper helper ──────────────────────────────────────────────
        private static EmployeeResponseDto MapToResponseDto(Employee e) => new()
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
    }
}
