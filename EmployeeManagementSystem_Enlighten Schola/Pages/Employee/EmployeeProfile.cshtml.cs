using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeManagementSystem_Enlighten_Schola.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem_Enlighten_Schola.Pages.Employee
{
    public class EmployeeProfileModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeProfileModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public Core.Entities.Employee Employee { get; set; }
        public List<Core.Entities.Document> Documents { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("Role") != "Employee" ||
                HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToPage("/Index");
            }

            int empId = HttpContext.Session.GetInt32("EmployeeId").Value;

            Employee = await _context.Employees.FindAsync(empId);
            if (Employee == null)
                return NotFound();

            Documents = await _context.Documents
                .Where(d => d.EmployeeId == empId)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadDocumentAsync(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Employee" ||
                HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToPage("/Index");
            }

            var empId = HttpContext.Session.GetInt32("EmployeeId").Value;
            var document = await _context.Documents.FindAsync(id);

            if (document == null || document.EmployeeId != empId)
                return Unauthorized();

            var filePath = Path.Combine(_env.WebRootPath, document.FilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var contentType = "application/octet-stream"; 
            return File(memory, contentType, Path.GetFileName(filePath));
        }
    }
}
