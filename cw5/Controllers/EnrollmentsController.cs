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
using Microsoft.AspNetCore.Authorization;

namespace cw5.Controllers
{
    [Authorize(Roles = "employee")]
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16520;Integrated Security=True";
        private IStudentDbService _dbService;

        public EnrollmentsController(IStudentDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetEnrollments([FromServices] IEnrollmentsDal dbService)
        {
            var list = new List<Enrollment>();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from enrollment ";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var enrollment = new Enrollment();
                    enrollment.StartDate = dr["StartDate"].ToString();
                    enrollment.Semester = dr["Semester"].ToString();
                    //st.Study = dr["IdStudy"].ToString();
                    list.Add(enrollment);
                }
            }

            return Ok(list);
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            /*
             * 
            ----------------------CWICZENIE 1
            
            var st = new Student();
            st.FirstName = request.FirstName;
            st.BirthDate = request.Birthdate;
            st.LastName = request.LastName;
            st.Studies = request.Studies;

            DateTime now = DateTime.Now;

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();
                var tran = con.BeginTransaction();

                try
                {
                    com.CommandText = "select IdStudy from Studies where Name = @name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    com.Transaction = tran;
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        tran.Rollback();
                        return BadRequest("Studia nie istnieją");
                    }
                    int idstudies = (int)dr["IdStudy"];
                    dr.Close();

                    com.CommandText = "select IdEnrollment from Enrollment where IdStudy=@idstudies and semester=1";
                    com.Parameters.AddWithValue("idstudies", idstudies);

                    com.Transaction = tran;
                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        com.CommandText = "INSERT INTO Enrollment(Semester, IdStudy, StartDate) VALUES(@semester, @idstudy, @startdate)";
                        com.Parameters.AddWithValue("semester", 1);
                        com.Parameters.AddWithValue("idstudy", idstudies);
                        com.Parameters.AddWithValue("startdate", now);

                        com.Transaction = tran;
                        com.ExecuteNonQuery();

                        com.CommandText = "select IdEnrollment from Enrollment where IdStudy=@idstudies and semester=1";
                        com.Parameters.AddWithValue("idstudies", idstudies);

                        com.Transaction = tran;
                        dr = com.ExecuteReader();
                    }
                    int idenrollment = (int)dr["IdEnrollment"];
                    dr.Close();

                    // Dodanie studenta

                    com.CommandText = "select IndexNumber from Student where IndexNumber = @index";
                    com.Parameters.AddWithValue("index", request.IndexNumber);

                    com.Transaction = tran;
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return BadRequest("Student już istnieje w bazie!");
                        
                    } else
                    {
                        dr.Close();
                        com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES(@index, @fname, @lname, @bdate, @idenrollment)";
                        //com.Parameters.AddWithValue("index", request.IndexNumber);
                        com.Parameters.AddWithValue("fname", request.FirstName);
                        com.Parameters.AddWithValue("lname", request.LastName);
                        com.Parameters.AddWithValue("bdate", request.Birthdate);
                        com.Parameters.AddWithValue("idenrollment", idenrollment);

                        com.Transaction = tran;
                        com.ExecuteNonQuery();

                        tran.Commit();
                    }
                    
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    Console.WriteLine(exc);
                }
            }*/


            var response = _dbService.EnrollStudent(request);

            switch (response.Type)
            {
                case "Ok": return Ok(response.ResponseObject);
                case "BadRequest": return BadRequest(response.Message);
                default: return Problem(response.Message);
            }
        }
    }
}