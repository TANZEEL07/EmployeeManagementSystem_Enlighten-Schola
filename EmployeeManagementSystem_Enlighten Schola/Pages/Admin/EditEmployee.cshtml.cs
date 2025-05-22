using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeManagementSystem_Enlighten_Schola.Infrastructure.Data;
using EmployeeManagementSystem_Enlighten_Schola.Core.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System;

namespace EmployeeManagementSystem_Enlighten_Schola.Pages.Admin
{
    public class EditEmployeeModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EditEmployeeModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public EditEmployeeViewModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Index");
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            Input = new EditEmployeeViewModel
            {
                EmployeeId = emp.EmployeeId,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Email = emp.Email,
                Mobile = emp.Mobile,
                Gender = emp.Gender,
                DOB = emp.DOB,
                DOJ = emp.DOJ,
                Designation = emp.Designation,
                Address = emp.Address,
                City = emp.City,
                PinCode = emp.PinCode,
                IsActive = emp.IsActive,
                ProfilePhotoPath = emp.ProfilePhotoPath
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Index");
            if (!ModelState.IsValid)
                return Page();

            var emp = await _context.Employees.FindAsync(Input.EmployeeId);
            if (emp == null)
                return NotFound();

            emp.FirstName = Input.FirstName;
            emp.LastName = Input.LastName;
            emp.Email = Input.Email;
            emp.Mobile = Input.Mobile;
            emp.Gender = Input.Gender;
            emp.DOB = Input.DOB;
            emp.DOJ = Input.DOJ;
            emp.Designation = Input.Designation;
            emp.Address = Input.Address;
            emp.City = Input.City;
            emp.PinCode = Input.PinCode;
            emp.IsActive = Input.IsActive;

            if (Input.ProfilePhoto != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + "_" + Input.ProfilePhoto.FileName;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePhoto.CopyToAsync(fs);
                }

                emp.ProfilePhotoPath = "/uploads/" + fileName;
            }
            else
            {
                // Retain existing photo path if no new photo is uploaded
                emp.ProfilePhotoPath = emp.ProfilePhotoPath;
                Input.ProfilePhotoPath = emp.ProfilePhotoPath;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/Index");
        }

    }
}
