using CourseManager.Data;
using CourseManager.Models;
using CourseManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;

    public StudentController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> ConfirmEnrollment(int studentId, int courseId)
    {
        var student = await _context.Students
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == studentId && s.CourseId == courseId);

        var viewModel = new ConfirmEnrollment
        {
            StudentId = student.Id,
            CourseId = student.CourseId,
            StudentName = student.Name,
            CourseName = student.Course.Name,
            RoomNumber = student.Course.RoomNumber,
            StartDate = student.Course.StartDate,
            Instructor = student.Course.Instructor
        };

        return View(viewModel); 
    }

    [HttpPost]
    public async Task<IActionResult> SubmitResponse(int studentId, string response)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null) return NotFound();

        if (response == "Yes")
            student.Status = StudentStatus.EnrollmentConfirmed;
        else if (response == "No")
            student.Status = StudentStatus.EnrollmentDeclined;

        await _context.SaveChangesAsync();
        return RedirectToAction("Thankyou");
    }

    public IActionResult Thankyou()
    {
        return View();
    }
}