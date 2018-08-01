using SchoolApp.Models;
using System.Collections.Generic;

namespace SchoolApp.ViewModels
{
    public class TeacherIndexData
    {
        public IEnumerable<Teacher> Teachers { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Class> Classs { get; set; }
    }
}