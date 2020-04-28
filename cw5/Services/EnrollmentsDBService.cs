using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw5.Models;
using cw5.DAL;

namespace cw5.Services
{
    public class EnrollmentsDBService : IEnrollmentsDal
    {
        public IEnumerable<Enrollment> GetEnrollments()
        {
            //...sql con
            return null;
        }
    }
}
