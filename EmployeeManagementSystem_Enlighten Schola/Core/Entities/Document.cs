using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem_Enlighten_Schola.Core.Entities;

public class Document
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string Number { get; set; }
    public string FilePath { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    [ForeignKey("Employee")]
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
}