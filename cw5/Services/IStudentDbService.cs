using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw5.DTOs.Requests;
using cw5.Models;

namespace cw5.Services
{
    public interface IStudentDbService
    {
        Response EnrollStudent(EnrollStudentRequest request);
        Response PromoteStudents(PromotionRequest request);
    }
}
