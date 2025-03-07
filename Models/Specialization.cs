using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWpfApp.Models
{
    public class Specialization
    {
        public int SpecializationId { get; set; }  
        public string Name { get; set; }           
        public int FacultyId { get; set; }        

        public Faculty Faculty { get; set; }  
        public ICollection<Student> Students { get; set; } 
    }
}
