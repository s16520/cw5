using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw5.DTOs.Responses
{
    
    public class TokenResponse
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
