using Microsoft.EntityFrameworkCore;
using CourseManager.Models;

namespace CourseManager.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){}

    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //setting up the relationships b/w tables
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Students)
            .WithOne(s => s.Course)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // enum to string
        modelBuilder.Entity<Student>()
            .Property(s => s.Status)
            .HasConversion<string>();

        //data seeding
        modelBuilder.Entity<Course>().HasData(
            new Course
            {
                Id = 1,
                Name = "Machine Learning",
                Instructor = "Harjot Singh",
                StartDate = new DateTime(2025, 9, 1),
                RoomNumber = "1A20"
            },
            new Course
            {
                Id = 2,
                Name = "Game Development",
                Instructor = "John Doe",
                StartDate = new DateTime(2025, 9, 1),
                RoomNumber = "2B98"
            }
        );

        // data seeding
        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                Id = 1,
                Name = "Kakashi",
                Email = "kakashi@example.com",
                Status = StudentStatus.EnrollmentConfirmed,
                CourseId = 1
            },
            new Student
            {
                Id = 2,
                Name = "Naruto Uzumaki",
                Email = "Ned@example.com",
                Status = StudentStatus.EnrollmentDeclined,
                CourseId = 1
            },
            new Student
            {
                Id = 3,
                Name = "Itachi Uchiha",
                Email = "itachi@example.com",
                Status = StudentStatus.EnrollmentDeclined,
                CourseId = 2
            }
        );
    }
}