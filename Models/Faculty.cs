using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWpfApp.Models
{
    public class Faculty
    {
        public int FacultyId { get; set; }  
        public string Name { get; set; }   

        public ICollection<Specialization> Specializations { get; set; } 
    }
}
