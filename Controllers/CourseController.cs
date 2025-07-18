using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManager.Models;
using CourseManager.Data;
using CourseManager.Services;
using CourseManager.ViewModels;

namespace CourseManager.Controllers
{
    public class CourseController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public CourseController( ApplicationDbContext context, IEmailService emailService, ICookieService cookieService): base(cookieService)
        {
             _context = context;
             _emailService = emailService;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Students)
                .ToListAsync();

            CookieDataSetup();

            return View(courses);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            CookieDataSetup();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (!ModelState.IsValid)
            {
                LogModelErrors();
                return View(course);
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            _cookieService.SetFirstVisitCookie();

            CookieDataSetup();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(course);

            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Courses.Any(c => c.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Manage
        public async Task<IActionResult> Manage(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                Console.WriteLine($"Course with ID {courseId} not found.");
                return NotFound();
            }

            var viewModel = new ManageCourse
            {
                Course = course,
                Students = course.Students.ToList(),
                InviteNotSentCount = course.Students.Count(s => s.Status == StudentStatus.ConfirmationMessageNotSent),
                SentCount = course.Students.Count(s => s.Status == StudentStatus.ConfirmationMessageSent),
                ConfirmedCount = course.Students.Count(s => s.Status == StudentStatus.EnrollmentConfirmed),
                DeclinedCount = course.Students.Count(s => s.Status == StudentStatus.EnrollmentDeclined)
            };

            CookieDataSetup();

            return View(viewModel);
        }
        #endregion

        #region Add Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(int courseId, string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Manage", new { courseId });

            var student = new Student
            {
                Name = name,
                Email = email,
                CourseId = courseId,
                Status = StudentStatus.ConfirmationMessageNotSent
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("Manage", new { courseId });
        }
        #endregion

        #region Send Confirmations
        [HttpPost]
        public IActionResult SendConfirmations(int courseId)
        {
            var course = _context.Courses
                .Include(c => c.Students)
                .FirstOrDefault(c => c.Id == courseId);

            if (course == null)
                return RedirectToAction("Manage", new { courseId });

            var studentsToSend = course.Students
                .Where(s => s.Status == StudentStatus.ConfirmationMessageNotSent)
                .ToList();

            if (studentsToSend.Count == 0)
                return RedirectToAction("Manage", new { courseId });

            Console.WriteLine($"Sending confirmation emails to {studentsToSend.Count} students for course {course.Name}.");

            foreach (var student in studentsToSend)
            {
                var emailBody = CreateEmailBody(course, student);
                var emailSubject = CreateEmailSubject(course);
                _emailService.SendEmail(emailSubject, emailBody, student.Email);
                student.Status = StudentStatus.ConfirmationMessageSent;
            }

            _context.SaveChanges();

            return RedirectToAction("Manage", new { courseId });
        }
        #endregion

        #region Private Helper Methods

        private string CreateEmailBody(Course course, Student student)
        {
            return $"Your request to enroll in the course {course.Name} in room {course.RoomNumber} starting {course.StartDate} with instructor {course.Instructor}. \n" +
                   $"http://localhost:5180/Student/ConfirmEnrollment?studentId={student.Id}&courseId={course.Id} \nPlease confirm your enrollment by visiting the link.\n\n" +
            "Sincerely,\nThe Course Manager App";
        }

        private string CreateEmailSubject(Course course)
        {
            return $"Enrollment Confirmation for \"{course.Name}\" Required";
        }

        private void LogModelErrors()
        {
            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    Console.WriteLine($"Field: {modelState.Key} — Error: {error.ErrorMessage}");
                }
            }
        }
        #endregion

    }
}
