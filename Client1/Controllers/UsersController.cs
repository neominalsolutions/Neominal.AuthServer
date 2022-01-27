using Client1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client1.Controllers
{
    [Authorize(AuthenticationSchemes = "IdentityServerScheme")]
    public class UsersController : Controller
    {
        public async Task<IActionResult> Index()
        {
           var result =  await HttpContext.AuthenticateAsync("IdentityServerScheme");

            var model = new AuthenticationViewModel
            {
                AuthenticatedUserId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value,
                Claims = User.Claims.ToList(),
                Properties = result.Properties.Items
            };

            return View(model);
        }
    }
}
