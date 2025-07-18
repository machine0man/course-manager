using CourseManager.Models;
namespace CourseManager.ViewModels;

public class ManageCourse
{
    public Course Course { get; set; }
    public List<Student> Students { get; set; }
    public int InviteNotSentCount { get; set; }
    public int SentCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int DeclinedCount { get; set; }
}
