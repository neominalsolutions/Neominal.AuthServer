using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1.Controllers
{

    // ControllerName is ClaimType
    // ActionName is ClaimValue

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    public class InstructorsRegisteration : ControllerBase
    {

        [HttpPost("approval")]
        public IActionResult Approval()
        {
            return Ok("Approved");
        }


        [HttpPost("create")]
        public IActionResult Create()
        {
            return Ok("Created");
        }


        [HttpPost("deny")]
        public IActionResult Deny()
        {
            return Ok("Created");
        }
    }
}
