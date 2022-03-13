using IdentityServer.Dtos;
using IdentityServer.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)] // IdentityServer Access Token haberleşmesi için açtık. 
    public class UsersController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager; 

        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] RegisterRequestDto request)
        {

            if(ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(new ApplicationUser
                {
                    Email = request.Email,
                    UserName = request.Email
                }, request.Password);

                if (result.Succeeded)
                {
                    return Ok("Kullanıcı Kaydı başarılı");
                }
                else
                {
                    return BadRequest(result.Errors.Select(x => x.Description));
                }
                

            }


            return BadRequest();
        }

    }
}
