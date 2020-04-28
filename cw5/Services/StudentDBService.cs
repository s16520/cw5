using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw5.Models;
using cw5.DAL;

namespace cw5.Services
{
    public class StudentDBService : IStudentsDal
    {
        public IEnumerable<Student> GetStudents()
        {
            //...sql con
            return null;
        }

    }
}
