using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Models
{
    public enum Grade
    {
        A, B, C, D, E
    }
    public class Class
    {
        public int ClassID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }

        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}