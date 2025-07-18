using System.ComponentModel.DataAnnotations;
namespace CourseManager.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Instructor { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = new DateTime();

    [Required]
    [RegularExpression(@"^[0-9][A-Z][0-9]{2}$", ErrorMessage = "Room must be in format 9Z99")]
    public string RoomNumber { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}