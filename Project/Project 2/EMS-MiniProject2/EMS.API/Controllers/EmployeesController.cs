using EMS.API.DTOs;
using EMS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _service;

        public EmployeesController(EmployeeService service)
        {
            _service = service;
        }

        // GET /api/employees?search=&department=&status=&sortBy=name&sortDir=asc&page=1&pageSize=10
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] EmployeeQueryParams queryParams)
        {
            var result = await _service.GetAllAsync(queryParams);
            return Ok(result);
        }

        // GET /api/employees/dashboard
        [HttpGet("dashboard")]
        [Authorize]
        public async Task<IActionResult> GetDashboard()
        {
            var summary = await _service.GetDashboardSummaryAsync();
            return Ok(summary);
        }

        // GET /api/employees/{id}
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var emp = await _service.GetByIdAsync(id);
            if (emp == null)
                return NotFound(new { message = $"Employee with ID {id} not found." });
            return Ok(emp);
        }

        // POST /api/employees
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] EmployeeRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (created, error) = await _service.CreateAsync(dto);

            if (error != null)
                return Conflict(new { message = error });

            return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
        }

        // PUT /api/employees/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (updated, error) = await _service.UpdateAsync(id, dto);

            if (error == "Employee not found.")
                return NotFound(new { message = error });

            if (error != null)
                return Conflict(new { message = error });

            return Ok(updated);
        }

        // DELETE /api/employees/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _service.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = error });
            return Ok(new { message = "Employee deleted successfully." });
        }
    }
}
