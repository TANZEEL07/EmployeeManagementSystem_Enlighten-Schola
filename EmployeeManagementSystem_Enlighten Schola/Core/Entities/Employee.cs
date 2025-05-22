using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace EmployeeManagementSystem_Enlighten_Schola.Core.Entities;

public class Employee
{
    [Key]
    public int EmployeeId { get; set; }
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
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PinCode { get; set; }
    public string ProfilePhotoPath { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public ICollection<Document> Documents { get; set; }
}
