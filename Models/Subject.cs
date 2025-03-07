using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWpfApp.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }  
        public string Name { get; set; }   
        public string Description { get; set; } 

        public ICollection<StudentSubject> StudentSubjects { get; set; } 
    }
}
