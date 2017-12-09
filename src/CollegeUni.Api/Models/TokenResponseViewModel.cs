using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Api.Models
{
    public class TokenResponseViewModel
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }
}
