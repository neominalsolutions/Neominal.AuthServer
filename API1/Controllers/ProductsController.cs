﻿using API1.Attributes;
using API1.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace API1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {



        //[TypeFilter(typeof(PermissionFilterAttribute),
        //  Arguments = new object[] { "update-profile" })]




        [Authorize(Policy ="PermissionPolicy")]
        [HttpGet("list")]
        public IActionResult GetProducts()
        {


            var model = new List<ProductDto>
            {
                new ProductDto
                {
                    Name = "Product1",
                    Price = 10,
                    Stock = 20
                },
                new ProductDto
                {
                    Name = "Product2",
                    Price = 11,
                    Stock= 30
                }
            };

            return Ok(model);
        }
    }
}
