namespace SchoolApp.Migrations
{
    using SchoolApp.DAL;
    using SchoolApp.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DAL.SchoolContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SchoolContext context)
        {
            var students = new List<Student>
            {
                new Student { FirstName = "Carson",   LastName = "Alexander",
                    ClassDate = DateTime.Parse("2010-09-01"), Gender = Gender.Male, Starosta = Master.Yes },
                new Student { FirstName = "Meredith", LastName = "Alonso",
                    ClassDate = DateTime.Parse("2012-09-01"), Gender = Gender.Female },
                new Student { FirstName = "Arturo",   LastName = "Anand",
                    ClassDate = DateTime.Parse("2013-09-01"), Gender = Gender.Male},
                new Student { FirstName = "Gytis",    LastName = "Barzdukas",
                    ClassDate = DateTime.Parse("2012-09-01"), Gender = Gender.Male },
                new Student { FirstName = "Yan",      LastName = "Li",
                    ClassDate = DateTime.Parse("2012-09-01"), Gender = Gender.Male, Starosta = Master.Yes },
                new Student { FirstName = "Peggy",    LastName = "Justice",
                    ClassDate = DateTime.Parse("2011-09-01"), Gender = Gender.Female },
                new Student { FirstName = "Laura",    LastName = "Norman",
                    ClassDate = DateTime.Parse("2013-09-01"), Gender = Gender.Female },
                new Student { FirstName = "Nino",     LastName = "Olivetto",
                    ClassDate = DateTime.Parse("2005-09-01"), Gender = Gender.Female }
            };

            students.ForEach(s => context.Students.AddOrUpdate(p => p.LastName, s));
            context.SaveChanges();

            var Teachers = new List<Teacher>
            {
                new Teacher { FirstName = "Kim",     LastName = "Abercrombie",
                    HireDate = DateTime.Parse("1995-03-11"), Gender=Gender.Male, Director = Master.Yes },
                new Teacher { FirstName = "Fadi",    LastName = "Fakhouri",
                    HireDate = DateTime.Parse("2002-07-06"), Gender=Gender.Female },
                new Teacher { FirstName = "Roger",   LastName = "Harui",
                    HireDate = DateTime.Parse("1998-07-01"), Gender=Gender.Female, HeadTeacher = Master.Yes },
                new Teacher { FirstName = "Candace", LastName = "Kapoor",
                    HireDate = DateTime.Parse("2001-01-15"), Gender=Gender.Male },
                new Teacher { FirstName = "Roger",   LastName = "Zheng",
                    HireDate = DateTime.Parse("2004-02-12"), Gender=Gender.Male }
            };
            Teachers.ForEach(s => context.Teachers.AddOrUpdate(p => p.LastName, s));
            context.SaveChanges();

            var departments = new List<Department>
            {
                new Department { Name = "English",     Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    TeacherID  = Teachers.Single( i => i.LastName == "Abercrombie").ID },
                new Department { Name = "Mathematics", Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    TeacherID  = Teachers.Single( i => i.LastName == "Fakhouri").ID },
                new Department { Name = "Engineering", Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    TeacherID  = Teachers.Single( i => i.LastName == "Harui").ID },
                new Department { Name = "Economics",   Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    TeacherID  = Teachers.Single( i => i.LastName == "Kapoor").ID }
            };
            departments.ForEach(s => context.Departments.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var courses = new List<Course>
            {
                new Course {CourseID = 1050, Title = "Chemistry",      Cost = 3,
                  DepartmentID = departments.Single( s => s.Name == "Engineering").DepartmentID,
                  Teachers = new List<Teacher>()
                },
                new Course {CourseID = 4022, Title = "Microeconomics", Cost = 3,
                  DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID,
                  Teachers = new List<Teacher>()
                },
                new Course {CourseID = 4041, Title = "Macroeconomics", Cost = 3,
                  DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID,
                  Teachers = new List<Teacher>()
                },
                new Course {CourseID = 1045, Title = "Calculus",       Cost = 4,
                  DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID,
                  Teachers = new List<Teacher>()
                },
                new Course {CourseID = 3141, Title = "Trigonometry",   Cost = 4,
                  DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID,
                  Teachers = new List<Teacher>()
                },
                new Course {CourseID = 2021, Title = "Composition",    Cost = 3,
                  DepartmentID = departments.Single( s => s.Name == "English").DepartmentID,
                  Teachers = new List<Teacher>()
                },
                new Course {CourseID = 2042, Title = "Literature",     Cost = 4,
                  DepartmentID = departments.Single( s => s.Name == "English").DepartmentID,
                  Teachers = new List<Teacher>()
                },
            };
            courses.ForEach(s => context.Courses.AddOrUpdate(p => p.CourseID, s));
            context.SaveChanges();

            var officeAssignments = new List<OfficeAssignment>
            {
                new OfficeAssignment {
                    TeacherID = Teachers.Single( i => i.LastName == "Fakhouri").ID,
                    Location = "Smith 17" },
                new OfficeAssignment {
                    TeacherID = Teachers.Single( i => i.LastName == "Harui").ID,
                    Location = "Gowan 27" },
                new OfficeAssignment {
                    TeacherID = Teachers.Single( i => i.LastName == "Kapoor").ID,
                    Location = "Thompson 304" },
            };
            officeAssignments.ForEach(s => context.OfficeAssignments.AddOrUpdate(p => p.TeacherID, s));
            context.SaveChanges();

            AddOrUpdateTeacher(context, "Chemistry", "Kapoor");
            AddOrUpdateTeacher(context, "Chemistry", "Harui");
            AddOrUpdateTeacher(context, "Microeconomics", "Zheng");
            AddOrUpdateTeacher(context, "Macroeconomics", "Zheng");

            AddOrUpdateTeacher(context, "Calculus", "Fakhouri");
            AddOrUpdateTeacher(context, "Trigonometry", "Harui");
            AddOrUpdateTeacher(context, "Composition", "Abercrombie");
            AddOrUpdateTeacher(context, "Literature", "Abercrombie");

            context.SaveChanges();

            var Clases = new List<Class>
            {
                new Class {
                    StudentID = students.Single(s => s.LastName == "Alexander").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                    Grade = Grade.A
                },
                 new Class {
                    StudentID = students.Single(s => s.LastName == "Alexander").ID,
                    CourseID = courses.Single(c => c.Title == "Microeconomics" ).CourseID,
                    Grade = Grade.C
                 },
                 new Class {
                    StudentID = students.Single(s => s.LastName == "Alexander").ID,
                    CourseID = courses.Single(c => c.Title == "Macroeconomics" ).CourseID,
                    Grade = Grade.B
                 },
                 new Class {
                     StudentID = students.Single(s => s.LastName == "Alonso").ID,
                    CourseID = courses.Single(c => c.Title == "Calculus" ).CourseID,
                    Grade = Grade.B
                 },
                 new Class {
                     StudentID = students.Single(s => s.LastName == "Alonso").ID,
                    CourseID = courses.Single(c => c.Title == "Trigonometry" ).CourseID,
                    Grade = Grade.B
                 },
                 new Class {
                    StudentID = students.Single(s => s.LastName == "Alonso").ID,
                    CourseID = courses.Single(c => c.Title == "Composition" ).CourseID,
                    Grade = Grade.B
                 },
                 new Class {
                    StudentID = students.Single(s => s.LastName == "Anand").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID
                 },
                 new Class {
                    StudentID = students.Single(s => s.LastName == "Anand").ID,
                    CourseID = courses.Single(c => c.Title == "Microeconomics").CourseID,
                    Grade = Grade.B
                 },
                new Class {
                    StudentID = students.Single(s => s.LastName == "Barzdukas").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry").CourseID,
                    Grade = Grade.B
                 },
                 new Class {
                    StudentID = students.Single(s => s.LastName == "Li").ID,
                    CourseID = courses.Single(c => c.Title == "Composition").CourseID,
                    Grade = Grade.B
                 },
                 new Class {
                    StudentID = students.Single(s => s.LastName == "Justice").ID,
                    CourseID = courses.Single(c => c.Title == "Literature").CourseID,
                    Grade = Grade.B
                 }
            };

            foreach (Class e in Clases)
            {
                var ClassInDataBase = context.Clases.Where(
                    s =>
                         s.Student.ID == e.StudentID &&
                         s.Course.CourseID == e.CourseID).SingleOrDefault();
                if (ClassInDataBase == null)
                {
                    context.Clases.Add(e);
                }
            }
            context.SaveChanges();
        }

        void AddOrUpdateTeacher(SchoolContext context, string courseTitle, string TeacherName)
        {
            var crs = context.Courses.SingleOrDefault(c => c.Title == courseTitle);
            var inst = crs.Teachers.SingleOrDefault(i => i.LastName == TeacherName);
            if (inst == null)
                crs.Teachers.Add(context.Teachers.Single(i => i.LastName == TeacherName));
        }
    }
}
