using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace cw5.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        public string IndexNumber { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Name Required !")]
        [MaxLength(10)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }

        [Required]
        public string Studies { get; set; }
    }
}
