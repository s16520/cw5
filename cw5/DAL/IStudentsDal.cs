using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw5.Models;

namespace cw5.DAL
{
    public interface IStudentsDal
    {
        public IEnumerable<Student> GetStudents();
    }
}
