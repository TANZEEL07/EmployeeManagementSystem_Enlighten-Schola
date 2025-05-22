using EmployeeManagementSystem_Enlighten_Schola;
using EmployeeManagementSystem_Enlighten_Schola.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem_Enlighten_Schola.Pages.Admin
{
    public class AddEmployeeModel : PageModel
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public AddEmployeeModel(AppDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        [BindProperty]
        public EmployeeModel employeeModel { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                RedirectToPage("/Index");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Index");
            if (!ModelState.IsValid)
                return Page();

            var existingUser = await _dbContext.Employees
        .FirstOrDefaultAsync(e =>
            e.UserName == $"{employeeModel.FirstName.ToLower()}_{employeeModel.LastName.ToLower()}"
            || e.Email == employeeModel.Email
            || e.Mobile == employeeModel.Mobile);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Username, Email, or Mobile number already exists.");
                return Page();
            }

            string profilePath = await SaveImageAsync(employeeModel.ProfilePhoto);

            var employee = new Core.Entities.Employee
            {
                FirstName = employeeModel.FirstName,
                LastName = employeeModel.LastName,
                Gender = employeeModel.Gender,
                DOB = DateTime.SpecifyKind(employeeModel.DOB, DateTimeKind.Utc),
                DOJ = DateTime.SpecifyKind(employeeModel.DOJ, DateTimeKind.Utc),
                Mobile = employeeModel.Mobile,
                Email = employeeModel.Email,
                Designation = employeeModel.Designation,
                Address = employeeModel.Address,
                City = employeeModel.City,
                PinCode = employeeModel.PinCode,
                UserName = $"{employeeModel.FirstName.ToLower()}_{employeeModel.LastName.ToLower()}",
                Password = GenerateRandomPassword(8),
                ProfilePhotoPath = profilePath,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Admin/Index");
        }

        private async Task<string> SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{uniqueFileName}";
        }

        private string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[random.Next(chars.Length)];
            }
            return new string(buffer);
        }

        public class EmployeeModel
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            public string Gender { get; set; }

            public DateTime DOB { get; set; }

            public DateTime DOJ { get; set; }

            public string Mobile { get; set; }

            public string Email { get; set; }

            public string Designation { get; set; }

            public string Address { get; set; }

            public string City { get; set; }

            public string PinCode { get; set; }

            public IFormFile ProfilePhoto { get; set; }
        }
    }
}
