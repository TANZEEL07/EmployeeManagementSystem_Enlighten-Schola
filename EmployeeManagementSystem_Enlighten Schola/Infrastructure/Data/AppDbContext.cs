using EmployeeManagementSystem_Enlighten_Schola.Core.Entities;
using Microsoft.EntityFrameworkCore;
namespace EmployeeManagementSystem_Enlighten_Schola.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Document> Documents { get; set; }

}
