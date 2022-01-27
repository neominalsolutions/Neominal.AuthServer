using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client1.Models
{
    public class AuthenticationViewModel
    {
        public string AuthenticatedUserId { get; set; }
        public List<Claim> Claims { get; set; }
        public IDictionary<string,string> Properties { get; set; }
    }
}
