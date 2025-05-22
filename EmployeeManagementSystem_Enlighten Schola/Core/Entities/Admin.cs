using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem_Enlighten_Schola.Core.Entities;

public class Admin
{
    [Key]
    public int AdminId { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}
