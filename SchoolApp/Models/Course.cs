using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApp.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(0, 5)]
        public int Cost { get; set; }

        public int DepartmentID { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Class> Classs { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}