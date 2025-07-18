using System.ComponentModel.DataAnnotations;
namespace CourseManager.Models;

public class Student
{
	public int Id { get; set; }

	[Required]
	public string Name { get; set; } = string.Empty;

	[Required]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;
	[Required]
	public StudentStatus Status { get; set; } = StudentStatus.ConfirmationMessageNotSent;


	//Foreign key
	public int CourseId { get; set; }

	public Course Course { get; set; }
}
// Enum for student status
public enum StudentStatus
{
	ConfirmationMessageNotSent = 0,
	ConfirmationMessageSent,
	EnrollmentConfirmed,
	EnrollmentDeclined
}