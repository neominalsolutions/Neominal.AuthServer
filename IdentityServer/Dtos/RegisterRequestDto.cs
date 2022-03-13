using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Dtos
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage ="E-PoSta zorunlu")]
        [EmailAddress(ErrorMessage ="E-Posta formatında giriniz")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Parola zorunlu")]
        public string Password { get; set; }

    }
}
