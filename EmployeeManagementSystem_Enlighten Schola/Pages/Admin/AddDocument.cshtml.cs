using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using System;
using EmployeeManagementSystem_Enlighten_Schola.Infrastructure.Data;
using EmployeeManagementSystem_Enlighten_Schola.Core.Entities;

namespace EmployeeManagementSystem_Enlighten_Schola.Pages.Admin
{
    public class AddDocumentModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AddDocumentModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty(SupportsGet = true)]
        public int EmployeeId { get; set; }

        [BindProperty]
        public string DocumentName { get; set; }

        [BindProperty]
        public string Number { get; set; }  // Added property for Document Number

        [BindProperty]
        public IFormFile DocumentFile { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Index");
            var emp = await _context.Employees.FindAsync(EmployeeId);
            if (emp == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Index");
            if (DocumentFile == null || DocumentFile.Length == 0)
                ModelState.AddModelError("DocumentFile", "Please select a document file.");

            if (string.IsNullOrEmpty(DocumentName))
                ModelState.AddModelError("DocumentName", "Please enter document name.");

            if (string.IsNullOrEmpty(Number))
                ModelState.AddModelError("Number", "Please enter document number.");

            if (!ModelState.IsValid)
                return Page();

            var uploadsFolder = Path.Combine(_env.WebRootPath, "employee-documents");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(DocumentFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await DocumentFile.CopyToAsync(fs);
            }

            var document = new Document
            {
                EmployeeId = EmployeeId,
                Number = Number,                
                Name = DocumentName,
                FilePath = "/employee-documents/" + uniqueFileName,
                UploadedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/ViewEmployee", new { id = EmployeeId });
        }
    }
}
