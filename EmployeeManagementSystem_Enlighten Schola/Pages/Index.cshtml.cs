using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeManagementSystem_Enlighten_Schola.Infrastructure.Data;

namespace EmployeeManagementSystem_Enlighten_Schola.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppDbContext _context;

    public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [BindProperty]
    public string UserName { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter username and password.";
            return Page();
        }

        var admin = _context.Admins.FirstOrDefault(a => a.UserName == UserName);
        if (admin != null)
        {
            if (admin.Password == Password)
            {
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetInt32("AdminId", admin.AdminId);
                return RedirectToPage("/Admin/Index");
            }
            else
            {
                ErrorMessage = "Invalid password.";
                return Page();
            }
        }

        var emp = _context.Employees.FirstOrDefault(e => e.UserName == UserName);
        if (emp != null)
        {
            if (emp.Password == Password)
            {
                HttpContext.Session.SetString("Role", "Employee");
                HttpContext.Session.SetInt32("EmployeeId", emp.EmployeeId);
                return RedirectToPage("/Employee/EmployeeProfile");
            }
            else
            {
                ErrorMessage = "Invalid password.";
                return Page();
            }
        }

        ErrorMessage = "User not found.";
        return Page();
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Clear(); 
        return RedirectToPage("/Index");
    }
}
