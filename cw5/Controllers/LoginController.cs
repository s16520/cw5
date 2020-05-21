using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using cw5.DAL;
using cw5.DTOs.Requests;
using cw5.DTOs.Responses;
using cw5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace cw5.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : Controller
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16520;Integrated Security=True";

        private IStudentsDal _dbService;

        public IConfiguration Configuration { get; set; }
        public LoginController(IStudentsDal dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
        }

        public static string CreatePswd(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                    password: value,
                                    salt: Encoding.UTF8.GetBytes(salt),
                                    prf: KeyDerivationPrf.HMACSHA512,
                                    iterationCount: 10000,
                                    numBytesRequested: 256 / 8
                );
            return Convert.ToBase64String(valueBytes);
        }

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static bool Validate(string value, string salt, string hash)
            => CreatePswd(value, salt) == hash;

        [HttpPost]
        public IActionResult Login(LoginRequestDto request)
        {
            var st = new Student();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student where Student.IndexNumber = @index";

                com.Parameters.AddWithValue("index", request.Login);

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = (DateTime)dr["BirthDate"];
                    st.Salt = dr["Salt"].ToString();
                    st.Password = dr["Password"].ToString();
                }

                if (st == null || !Validate(request.Haslo, st.Salt, st.Password))  //
                    return NotFound("Authorization failed");

                string role;

                if (st.IndexNumber.StartsWith("s"))
                    role = "student";
                else
                    role = "employee";


                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, st.IndexNumber),
                    new Claim(ClaimTypes.Role, role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                string refreshToken = Guid.NewGuid().ToString();

                dr.Close();
                com.CommandText = "UPDATE Student set RefreshToken = @refreshToken where Student.IndexNumber = @index";
                com.Parameters.AddWithValue("refreshToken", refreshToken);

                com.ExecuteNonQuery();
                dr.Close();
                TokenResponse resp = new TokenResponse();
                resp.accessToken = accessToken;
                resp.refreshToken = refreshToken;

                return Ok(resp);
            }
        }


        [HttpPost("refresh-token/{refToken}")]
        public IActionResult RefreshToken(string refToken)
        {
            if (refToken == null)
                return NotFound("Authorization failed");

            var st = new Student();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student where Student.RefreshToken = @refreshToken";

                com.Parameters.AddWithValue("refreshToken", refToken);

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = (DateTime)dr["BirthDate"];
                }

                string role;

                if (st.IndexNumber.StartsWith("s"))
                    role = "student";
                else
                    role = "employee";


                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, st.IndexNumber),
                    new Claim(ClaimTypes.Role, role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                string newRefreshToken = Guid.NewGuid().ToString();

                dr.Close();
                com.CommandText = "UPDATE Student set RefreshToken = @newRefreshToken where Student.IndexNumber = @index";
                com.Parameters.AddWithValue("newRefreshToken", newRefreshToken);
                com.Parameters.AddWithValue("index", st.IndexNumber);

                com.ExecuteNonQuery();
                dr.Close();

                TokenResponse resp = new TokenResponse();
                resp.accessToken = accessToken;
                resp.refreshToken = newRefreshToken;

                return Ok(resp);
            }
        }


        [Authorize]
        [HttpPost("changePswd")]
        public IActionResult ChangePswd(LoginRequestDto request)
        {
            var st = new Student();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student where Student.IndexNumber = @index";

                com.Parameters.AddWithValue("index", request.Login);

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = (DateTime)dr["BirthDate"];
                    st.Password = dr["Salt"].ToString();
                    st.Password = dr["Password"].ToString();
                }

                if (st == null)
                    return NotFound("Authorization failed");

                var s = CreateSalt();
                var p = CreatePswd(request.Haslo, s);

                dr.Close();
                com.CommandText = "UPDATE Student Set Password = @newpswd, Salt = @newsalt WHERE IndexNumber = @index ";
                com.Parameters.AddWithValue("newpswd", p);
                com.Parameters.AddWithValue("newsalt", s);

                com.ExecuteNonQuery();
                dr.Close();

                return Ok("New password added");

            }

        }
    }
}