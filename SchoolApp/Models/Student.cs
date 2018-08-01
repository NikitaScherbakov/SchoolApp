using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Models
{
    public enum Master
    {
        Yes, No
    }

    public class Student : Person
    {        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Start")]
        public DateTime? ClassDate { get; set; }

        public Master? Starosta { get; set; }

        public virtual ICollection<Class> Clases { get; set; }
    }
}