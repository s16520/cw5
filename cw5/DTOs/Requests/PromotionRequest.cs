using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw5.DTOs.Requests
{
    public class PromotionRequest
    {
        [Required(ErrorMessage = "Podaj Studia !")]
        [MaxLength(100)]
        public string Studies { get; set; }

        [Required(ErrorMessage ="Podaj semestr !")]
        public int Semester { get; set; }
    }
}
