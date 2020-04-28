using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw5.DTOs.Requests;
using cw5.DTOs.Responses;
using cw5.Models;

namespace cw5.Services
{
    public class SqlServerDbService : IStudentDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16520;Integrated Security=True";

        public Response EnrollStudent(EnrollStudentRequest request)
        {
            var st = new Student();
            st.FirstName = request.FirstName;
            st.BirthDate = request.Birthdate;
            st.LastName = request.LastName;
            st.Studies = request.Studies;

            DateTime now = DateTime.Now;

            var enrollment = new EnrollStudentResponse();
            enrollment.Semester = 1;
            enrollment.StartDate = now;
            enrollment.LastName = request.LastName;

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
                        dr.Close();
                        tran.Rollback();
                        return new Response("NotFound", "Studia nie znalezione!");
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
                        dr.Close();

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
                        return new Response("BadResponse","Student już istnieje w bazie!");

                    }
                    else
                    {
                        dr.Close();
                        com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES(@index, @fname, @lname, @bdate, @idenrollment)";
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

                return new Response("Ok","Dodano studenta!", enrollment);
            }

        }

        public Response PromoteStudents(PromotionRequest request)
        {
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
                        dr.Close();
                        tran.Rollback();
                        return new Response("NotFound", "Studia nie znalezione!");
                    }
                    dr.Close();

                    com.CommandText = "select IdEnrollment from Enrollment inner join Studies on Enrollment.IdStudy=Studies.IdStudy" +
                        " where Studies.Name = @name and Enrollment.Semester = 1";

                    com.Transaction = tran;
                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return new Response("NotFound", "Wpis na studia nie odnaleziony!");
                    }
                    dr.Close();

                    com.Parameters.Clear();
                    com.CommandText = "dbo.PromoteStudents";
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add(new SqlParameter("@Studies",request.Studies));
                    com.Parameters.Add(new SqlParameter("@Semester", request.Semester));
                   
                    com.ExecuteNonQuery();
                   
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    Console.WriteLine(exc);
                    return new Response("ERR", exc.ToString());
                }
                finally
                {
                    con.Close();
                }

                PromotionResponse promotion = new PromotionResponse();
                promotion.Semester = request.Semester + 1;
                promotion.Studies = request.Studies;

                return new Response("Ok","PROMOTIONS: Proceed succesfly", promotion);
            }
        }
     
    }
}
