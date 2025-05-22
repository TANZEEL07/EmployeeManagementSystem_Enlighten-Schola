using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeManagementSystem_Enlighten_Schola.Infrastructure.Data;
using EmployeeManagementSystem_Enlighten_Schola;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem_Enlighten_Schola.Pages.Admin
{
    public class ViewEmployeeModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ViewEmployeeModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Core.Entities.Employee Employee { get; set; }
        public List<Core.Entities.Document> Documents { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Index");
            Employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == Id);
            if (Employee == null)
                return NotFound();

            Documents = await _context.Documents
                .Where(d => d.EmployeeId == Id)
                .OrderBy(d => d.Number)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteDocumentAsync(int id, int employeeId)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Index");
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                var filePath = Path.Combine(_env.WebRootPath, document.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = employeeId });
        }

        public async Task<IActionResult> OnGetDownloadDocumentAsync(int id)
        {

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return NotFound();

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
