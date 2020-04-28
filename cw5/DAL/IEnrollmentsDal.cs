using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw5.Models;

namespace cw5.DAL
{
    public interface IEnrollmentsDal
    {
        public IEnumerable<Enrollment> GetEnrollments();
    }
}
