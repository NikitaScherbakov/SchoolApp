using SchoolApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SchoolApp.DAL
{
    public class SchoolInitializer : DropCreateDatabaseIfModelChanges<SchoolContext>
    {
        protected override void Seed(SchoolContext context)
        {
            var students = new List<Student>
            {
            new Student{FirstName="Carson",LastName="Alexander",ClassDate=DateTime.Parse("2005-09-01"),Gender=Gender.Male},
            new Student{FirstName="Meredith",LastName="Alonso",ClassDate=DateTime.Parse("2002-09-01"),Gender=Gender.Female},
            new Student{FirstName="Arturo",LastName="Anand",ClassDate=DateTime.Parse("2003-09-01"),Gender=Gender.Male},
            new Student{FirstName="Gytis",LastName="Barzdukas",ClassDate=DateTime.Parse("2002-09-01"),Gender=Gender.Male},
            new Student{FirstName="Yan",LastName="Li",ClassDate=DateTime.Parse("2002-09-01"),Gender=Gender.Male},
            new Student{FirstName="Peggy",LastName="Justice",ClassDate=DateTime.Parse("2001-09-01"),Gender=Gender.Female},
            new Student{FirstName="Laura",LastName="Norman",ClassDate=DateTime.Parse("2003-09-01"),Gender=Gender.Female},
            new Student{FirstName="Nino",LastName="Olivetto",ClassDate=DateTime.Parse("2005-09-01"),Gender=Gender.Female}
            };

            students.ForEach(s => context.Students.Add(s));
            context.SaveChanges();

            var courses = new List<Course>
            {
            new Course{CourseID=1050,Title="Chemistry",Cost=3,},
            new Course{CourseID=4022,Title="Microeconomics",Cost=3,},
            new Course{CourseID=4041,Title="Macroeconomics",Cost=3,},
            new Course{CourseID=1045,Title="Calculus",Cost=4,},
            new Course{CourseID=3141,Title="Trigonometry",Cost=4,},
            new Course{CourseID=2021,Title="Composition",Cost=3,},
            new Course{CourseID=2042,Title="Literature",Cost=4,}
            };
            courses.ForEach(s => context.Courses.Add(s));
            context.SaveChanges();
            
            var Clases = new List<Class>
            {
            new Class{StudentID=1,CourseID=1050,Grade=Grade.A},
            new Class{StudentID=1,CourseID=4022,Grade=Grade.C},
            new Class{StudentID=1,CourseID=4041,Grade=Grade.B},
            new Class{StudentID=2,CourseID=1045,Grade=Grade.B},
            new Class{StudentID=2,CourseID=3141,Grade=Grade.E},
            new Class{StudentID=2,CourseID=2021,Grade=Grade.E},
            new Class{StudentID=3,CourseID=1050},
            new Class{StudentID=4,CourseID=1050,},
            new Class{StudentID=4,CourseID=4022,Grade=Grade.E},
            new Class{StudentID=5,CourseID=4041,Grade=Grade.C},
            new Class{StudentID=6,CourseID=1045},
            new Class{StudentID=7,CourseID=3141,Grade=Grade.A},
            };
            Clases.ForEach(s => context.Clases.Add(s));
            context.SaveChanges();
        }
    }
}