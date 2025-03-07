using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWpfApp.Models
{
    public class Student
    {
        public int StudentId { get; set; } 
        public string Name { get; set; }   
        public int SpecializationId { get; set; } 

        
        public Specialization Specialization { get; set; } 
        public ICollection<StudentSubject> StudentSubjects { get; set; } 
    }
}
