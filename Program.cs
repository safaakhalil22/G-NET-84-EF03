using System;
using G_NET_84_EF03.Models;
using Microsoft.EntityFrameworkCore;

namespace G_NET_84_EF03
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            APPDbContext dbContext = new APPDbContext();

            await SeedStudentsAndEnrollments(dbContext);

            await AddInstructor(dbContext, "Laila Nabil", "laila.nabil@learnhub.com", "Cloud and DevOps instructor.");
            await AddCourse(dbContext, "Docker for Developers", 520m, CourseLevel.Intermediate, 1, 3);
            await AddCourse(dbContext, "Kotlin Basics", 300m, CourseLevel.Beginner, 99, 7);
            await AddStudents(dbContext);
            await EnrollStudent(dbContext, 1, 3);
            await EnrollStudent(dbContext, 1, 3);
            await EnrollStudent(dbContext, 2, 6);
            await EnrollStudent(dbContext, 99, 1);

            await FindCourse(dbContext, 2, "SQL Server Essentials");
            await FindCourse(dbContext, 99, "Not A Course");
            await FilterCourses(dbContext, 300m, 800m);
            await GetSubCategories(dbContext, 1);
            await GetCourseRoster(dbContext, 1);

            await ChangePrice(dbContext, 4, 399m);
            await UpdateProgress(dbContext, 1, 2, 70);
            await UpdateProgress(dbContext, 1, 2, 100);
            await UpdateProgress(dbContext, 1, 2, 150);
            await PublishCourse(dbContext, 5);

            await Unenroll(dbContext, 3, 4);
            await DeleteCategoryUnsafe(dbContext, 1);
            await DeleteCategorySafe(dbContext, 1);
            await DeleteCategorySafe(dbContext, 7);
            await DeleteCourse(dbContext, 3);

            await BulkPriceRaise(dbContext, CourseLevel.Intermediate);
            await BulkDeleteDroppedEnrollments(dbContext);
        }

        #region Data Seeding
        static async Task SeedStudentsAndEnrollments(APPDbContext dbContext)
        {
            if (!await dbContext.Students.AnyAsync())
            {
                dbContext.Students.AddRange(
                    new Student { FullName = "Mohamed Ali", Email = "mohamed.ali@mail.com", DateOfBirth = new DateTime(2000, 4, 12) },
                    new Student { FullName = "Nour Ibrahim", Email = "nour.ibrahim@mail.com", DateOfBirth = new DateTime(1999, 9, 3) },
                    new Student { FullName = "Youssef Adel", Email = "youssef.adel@mail.com", DateOfBirth = new DateTime(2001, 1, 25) },
                    new Student { FullName = "Mariam Samir", Email = "mariam.samir@mail.com", DateOfBirth = new DateTime(1998, 11, 30) },
                    new Student { FullName = "Karim Fathy", Email = "karim.fathy@mail.com", DateOfBirth = new DateTime(2002, 7, 8) });
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Students Data Seed");
            }

            if (!await dbContext.Enrollments.AnyAsync())
            {
                var studentIds = await dbContext.Students.OrderBy(S => S.Id).Select(S => S.Id).Take(5).ToListAsync();
                if (studentIds.Count < 5)
                {
                    Console.WriteLine("Not enough students to seed enrollments");
                    return;
                }

                dbContext.Enrollments.AddRange(
                    new Enrollment { StudentId = studentIds[0], CourseId = 1, EnrolledAt = DateTime.Now.AddDays(-60), Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 92.5m },
                    new Enrollment { StudentId = studentIds[0], CourseId = 2, EnrolledAt = DateTime.Now.AddDays(-40), Progress = 45, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = studentIds[1], CourseId = 1, EnrolledAt = DateTime.Now.AddDays(-55), Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 85m },
                    new Enrollment { StudentId = studentIds[1], CourseId = 3, EnrolledAt = DateTime.Now.AddDays(-30), Progress = 30, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = studentIds[2], CourseId = 2, EnrolledAt = DateTime.Now.AddDays(-35), Progress = 20, Status = EnrollmentStatus.Dropped },
                    new Enrollment { StudentId = studentIds[2], CourseId = 4, EnrolledAt = DateTime.Now.AddDays(-20), Progress = 60, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = studentIds[3], CourseId = 1, EnrolledAt = DateTime.Now.AddDays(-25), Progress = 10, Status = EnrollmentStatus.Dropped },
                    new Enrollment { StudentId = studentIds[4], CourseId = 4, EnrolledAt = DateTime.Now.AddDays(-50), Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 78m });
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Enrollments Data Seed");
            }
        }
        #endregion

        #region Create (C)
        #region Add An Instructor
        static async Task AddInstructor(APPDbContext dbContext, string fullName, string email, string? bio)
        {
            Console.WriteLine("--- Add An Instructor ---");
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Full name and email are required");
                return;
            }

            Instructor instructor = new Instructor()
            {
                FullName = fullName,
                Email = email,
                Bio = bio,
                JoinedAt = DateTime.Now
            };
            await dbContext.Instructors.AddAsync(instructor);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Instructor Added With Id = {instructor.Id}");
        }
        #endregion

        #region Add A Course
        static async Task AddCourse(APPDbContext dbContext, string title, decimal price, CourseLevel level, int instructorId, int categoryId)
        {
            Console.WriteLine("--- Add A Course ---");
            if (!await dbContext.Instructors.AnyAsync(I => I.Id == instructorId))
            {
                Console.WriteLine($"Instructor {instructorId} does not exist");
                return;
            }
            if (!await dbContext.Categories.AnyAsync(C => C.Id == categoryId))
            {
                Console.WriteLine($"Category {categoryId} does not exist");
                return;
            }
            if (price <= 0)
            {
                Console.WriteLine("Price must be greater than zero");
                return;
            }

            Course course = new Course()
            {
                Title = title,
                Price = price,
                Level = level,
                InstructorId = instructorId,
                CategoryId = categoryId
            };
            await dbContext.Courses.AddAsync(course);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Course Added With Id = {course.Id}");
        }
        #endregion

        #region Add Several Students
        static async Task AddStudents(APPDbContext dbContext)
        {
            Console.WriteLine("--- Add Several Students ---");
            Student[] students =
            {
                new Student { FullName = "Hana Tarek", Email = "hana.tarek@mail.com", DateOfBirth = new DateTime(2000, 2, 14) },
                new Student { FullName = "Ziad Mahmoud", Email = "ziad.mahmoud@mail.com", DateOfBirth = new DateTime(1997, 6, 21) },
                new Student { FullName = "Salma Reda", Email = "salma.reda@mail.com", DateOfBirth = new DateTime(2003, 12, 5) }
            };
            dbContext.Students.AddRange(students);
            await dbContext.SaveChangesAsync();
            foreach (var student in students)
            {
                Console.WriteLine($"{student.Id} - {student.FullName}");
            }
        }
        #endregion

        #region Enroll A Student
        static async Task EnrollStudent(APPDbContext dbContext, int studentId, int courseId)
        {
            Console.WriteLine("--- Enroll A Student ---");
            var student = await dbContext.Students.FindAsync(studentId);
            if (student is null)
            {
                Console.WriteLine($"Enrollment refused: student {studentId} does not exist");
                return;
            }
            var course = await dbContext.Courses.FindAsync(courseId);
            if (course is null)
            {
                Console.WriteLine($"Enrollment refused: course {courseId} does not exist");
                return;
            }
            if (!course.IsPublished)
            {
                Console.WriteLine($"Enrollment refused: course '{course.Title}' is not published");
                return;
            }
            if (await dbContext.Enrollments.FindAsync(studentId, courseId) is not null)
            {
                Console.WriteLine($"Enrollment refused: {student.FullName} is already enrolled in '{course.Title}'");
                return;
            }

            Enrollment enrollment = new Enrollment()
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = DateTime.Now,
                Progress = 0,
                Status = EnrollmentStatus.Active
            };
            await dbContext.Enrollments.AddAsync(enrollment);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"{student.FullName} enrolled in '{course.Title}'");
        }
        #endregion
        #endregion

        #region Read | Retrieve (R)
        #region Find A Course
        static async Task FindCourse(APPDbContext dbContext, int id, string title)
        {
            Console.WriteLine("--- Find A Course ---");
            var courseById = await dbContext.Courses.FindAsync(id);
            if (courseById is not null)
                Console.WriteLine($"By Id: {courseById.Id} - {courseById.Title}");
            else
                Console.WriteLine($"By Id: course {id} not found");

            var courseByTitle = await dbContext.Courses.FirstOrDefaultAsync(C => C.Title == title);
            if (courseByTitle is not null)
                Console.WriteLine($"By Title: {courseByTitle.Id} - {courseByTitle.Title}");
            else
                Console.WriteLine($"By Title: '{title}' not found");
        }
        #endregion

        #region Filter Courses
        static async Task FilterCourses(APPDbContext dbContext, decimal minPrice, decimal maxPrice)
        {
            Console.WriteLine("--- Filter Courses ---");
            if (minPrice > maxPrice)
            {
                Console.WriteLine("Minimum price cannot be greater than maximum price");
                return;
            }

            var courses = await dbContext.Courses
                .Where(C => C.Price >= minPrice && C.Price <= maxPrice)
                .OrderByDescending(C => C.Price)
                .Select(C => new { C.Title, C.Level, C.Price })
                .ToListAsync();

            if (courses.Count == 0)
            {
                Console.WriteLine("No courses in this price range");
                return;
            }
            foreach (var course in courses)
            {
                Console.WriteLine($"{course.Title} | {course.Level} | {course.Price}");
            }
        }
        #endregion

        #region Sub-Categories
        static async Task GetSubCategories(APPDbContext dbContext, int categoryId)
        {
            Console.WriteLine("--- Sub-Categories ---");
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category is null)
            {
                Console.WriteLine($"Category {categoryId} not found");
            }
            else
            {
                var subCategories = await dbContext.Categories
                    .Where(C => C.ParentCategoryId == categoryId)
                    .ToListAsync();
                Console.WriteLine($"Sub-categories of {category.Name}:");
                if (subCategories.Count == 0)
                    Console.WriteLine("None");
                foreach (var subCategory in subCategories)
                {
                    Console.WriteLine($"{subCategory.Id} - {subCategory.Name}");
                }
            }

            var topLevelCategories = await dbContext.Categories
                .Where(C => C.ParentCategoryId == null)
                .ToListAsync();
            Console.WriteLine("Top-level categories:");
            foreach (var topLevelCategory in topLevelCategories)
            {
                Console.WriteLine($"{topLevelCategory.Id} - {topLevelCategory.Name}");
            }
        }
        #endregion

        #region Course Roster
        static async Task GetCourseRoster(APPDbContext dbContext, int courseId)
        {
            Console.WriteLine("--- Course Roster ---");
            if (!await dbContext.Courses.AnyAsync(C => C.Id == courseId))
            {
                Console.WriteLine($"Course {courseId} not found");
                return;
            }

            var studentIds = await dbContext.Enrollments
                .Where(E => E.CourseId == courseId)
                .Select(E => E.StudentId)
                .ToListAsync();
            Console.WriteLine($"Enrolled students: {studentIds.Count}");

            var students = await dbContext.Students
                .Where(S => studentIds.Contains(S.Id))
                .ToListAsync();
            foreach (var student in students)
            {
                Console.WriteLine(student.FullName);
            }
        }
        #endregion
        #endregion

        #region Update (U)
        #region Change A Price
        static async Task ChangePrice(APPDbContext dbContext, int courseId, decimal newPrice)
        {
            Console.WriteLine("--- Change A Price ---");
            if (newPrice <= 0)
            {
                Console.WriteLine("Price must be greater than zero");
                return;
            }
            var course = await dbContext.Courses.FirstOrDefaultAsync(C => C.Id == courseId);
            if (course is null)
            {
                Console.WriteLine($"Course {courseId} not found");
                return;
            }

            decimal oldPrice = course.Price;
            course.Price = newPrice;
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"'{course.Title}' price changed from {oldPrice} to {course.Price}");
        }
        #endregion

        #region Update Progress
        static async Task UpdateProgress(APPDbContext dbContext, int studentId, int courseId, int progress)
        {
            Console.WriteLine("--- Update Progress ---");
            if (progress < 0 || progress > 100)
            {
                Console.WriteLine("Progress must be between 0 and 100");
                return;
            }
            var enrollment = await dbContext.Enrollments.FindAsync(studentId, courseId);
            if (enrollment is null)
            {
                Console.WriteLine($"Student {studentId} is not enrolled in course {courseId}");
                return;
            }

            enrollment.Progress = progress;
            if (progress == 100)
                enrollment.Status = EnrollmentStatus.Completed;
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Progress = {enrollment.Progress}% , Status = {enrollment.Status}");
        }
        #endregion

        #region Publish A Course
        static async Task PublishCourse(APPDbContext dbContext, int courseId)
        {
            Console.WriteLine("--- Publish A Course ---");
            var course = await dbContext.Courses.FindAsync(courseId);
            if (course is null)
            {
                Console.WriteLine($"Course {courseId} not found");
                return;
            }
            if (course.IsPublished)
            {
                Console.WriteLine($"'{course.Title}' is already published");
                return;
            }

            course.IsPublished = true;
            course.PublishedAt = DateTime.Now;
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"'{course.Title}' published at {course.PublishedAt}");
        }
        #endregion
        #endregion

        #region Delete | Remove (D)
        #region Unenroll
        static async Task Unenroll(APPDbContext dbContext, int studentId, int courseId)
        {
            Console.WriteLine("--- Unenroll ---");
            var enrollment = await dbContext.Enrollments.FindAsync(studentId, courseId);
            if (enrollment is null)
            {
                Console.WriteLine($"Student {studentId} is not enrolled in course {courseId}");
                return;
            }

            dbContext.Enrollments.Remove(enrollment);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Student {studentId} unenrolled from course {courseId}");
        }
        #endregion

        #region Delete A Category
        static async Task DeleteCategoryUnsafe(APPDbContext dbContext, int categoryId)
        {
            Console.WriteLine("--- Delete A Category (Unsafe) ---");
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category is null)
            {
                Console.WriteLine($"Category {categoryId} not found");
                return;
            }

            try
            {
                dbContext.Categories.Remove(category);
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Category '{category.Name}' deleted");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                dbContext.Entry(category).State = EntityState.Unchanged;
            }
        }

        static async Task DeleteCategorySafe(APPDbContext dbContext, int categoryId)
        {
            Console.WriteLine("--- Delete A Category (Safe) ---");
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category is null)
            {
                Console.WriteLine($"Category {categoryId} not found");
                return;
            }
            if (await dbContext.Categories.AnyAsync(C => C.ParentCategoryId == categoryId))
            {
                Console.WriteLine($"Category '{category.Name}' still has sub-categories and cannot be deleted");
                return;
            }
            if (await dbContext.Courses.AnyAsync(C => C.CategoryId == categoryId))
            {
                Console.WriteLine($"Category '{category.Name}' still has courses and cannot be deleted");
                return;
            }

            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Category '{category.Name}' deleted");
        }
        #endregion

        #region Delete A Course
        static async Task DeleteCourse(APPDbContext dbContext, int courseId)
        {
            Console.WriteLine("--- Delete A Course ---");
            var course = await dbContext.Courses.FindAsync(courseId);
            if (course is null)
            {
                Console.WriteLine($"Course {courseId} not found");
                return;
            }

            int before = await dbContext.Enrollments.CountAsync(E => E.CourseId == courseId);
            Console.WriteLine($"Enrollments before delete: {before}");

            dbContext.Courses.Remove(course);
            await dbContext.SaveChangesAsync();

            int after = await dbContext.Enrollments.CountAsync(E => E.CourseId == courseId);
            Console.WriteLine($"Enrollments after delete: {after}");
            
        }
        #endregion
        #endregion

        #region Bulk Operations
        #region Bulk Price Raise
        static async Task BulkPriceRaise(APPDbContext dbContext, CourseLevel level)
        {
            Console.WriteLine("--- Bulk Price Raise ---");
            int affectedRows = await dbContext.Courses
                .Where(C => C.IsPublished && C.Level == level)
                .ExecuteUpdateAsync(S => S.SetProperty(C => C.Price, C => C.Price * 1.1m));
            Console.WriteLine($"Affected rows: {affectedRows}");
        }
        #endregion

        #region Bulk Delete
        static async Task BulkDeleteDroppedEnrollments(APPDbContext dbContext)
        {
            Console.WriteLine("--- Bulk Delete ---");
            int affectedRows = await dbContext.Enrollments
                .Where(E => E.Status == EnrollmentStatus.Dropped)
                .ExecuteDeleteAsync();
            Console.WriteLine($"Affected rows: {affectedRows}");
        }
        #endregion
        #endregion
    }
}
