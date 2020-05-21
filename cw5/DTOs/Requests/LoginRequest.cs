using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw5.DTOs.Requests
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Login wymagany !")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Hasło wymagane !")]
        public string Haslo { get; set; }
    }
}
