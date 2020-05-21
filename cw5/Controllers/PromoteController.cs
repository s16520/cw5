using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

using cw5.DAL;
using cw5.Models;
using cw5.DTOs.Requests;
using cw5.DTOs.Responses;
using cw5.Services;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace cw5.Controllers
{
    [Route("api/enrollments/promotions")]
    [Authorize(Roles = "employee")]
    [ApiController]
    
    public class PromoteControlle : ControllerBase
    {
        
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16520;Integrated Security=True";
        private IStudentDbService _dbService;

        public PromoteControlle(IStudentDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public IActionResult PromoteStudents(PromotionRequest request)
        {
           
            var response = _dbService.PromoteStudents(request);

            switch (response.Type)
            {
                case "Ok": return Ok(response.ResponseObject);
                case "BadRequest": return BadRequest(response.Message);
                case "NotFound": return NotFound(response.Message);
                default: return Problem(response.Message);
            }

        }



    }
}
