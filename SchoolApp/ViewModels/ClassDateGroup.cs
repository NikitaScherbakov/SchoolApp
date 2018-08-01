using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolApp.ViewModels
{
    public class ClassDateGroup
    {
        [DataType(DataType.Date)]
        public DateTime? ClassDate { get; set; }

        public int StudentCount { get; set; }
    }
}