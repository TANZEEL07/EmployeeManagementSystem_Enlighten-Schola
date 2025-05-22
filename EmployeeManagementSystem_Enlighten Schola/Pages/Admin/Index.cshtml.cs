using EmployeeManagementSystem_Enlighten_Schola.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem_Enlighten_Schola.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Core.Entities.Employee> Employees { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Index");

            Employees = await _context.Employees.ToListAsync();
            return Page();
        }

        // Toggle employee status by EmployeeId (POST method)
        public async Task<IActionResult> OnPostToggleStatusAsync(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Index");
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.IsActive = !employee.IsActive;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
