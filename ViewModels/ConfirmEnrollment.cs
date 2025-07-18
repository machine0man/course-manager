namespace CourseManager.ViewModels;
public class ConfirmEnrollment
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }

    public string StudentName { get; set; }
    public string CourseName { get; set; }
    public string RoomNumber { get; set; }
    public DateTime StartDate { get; set; }
    public string Instructor { get; set; }
}
