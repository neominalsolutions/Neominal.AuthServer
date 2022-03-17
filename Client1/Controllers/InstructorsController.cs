using Client1.Utils.EndPoints;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client1.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly HttpClient _identityServer;
        private readonly HttpClient _resourceApi1;

        public InstructorsController(IHttpClientFactory httpClientFactory)
        {
            _identityServer = httpClientFactory.CreateClient("IdentityServer");
            _resourceApi1 = httpClientFactory.CreateClient("ResourceApi1");
        }

        public async Task<IActionResult> Approve()
        {

            var accessToken = await HttpContext.GetTokenAsync("MVCIdentityScheme", "access_token");
            _resourceApi1.SetBearerToken(accessToken);

            var result = await _resourceApi1.GetAsync(ResourceApi1EndPoint.InstructorApproveEndPoint);

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();

                return View();
            }

            return StatusCode(403);
        }


        public async Task<IActionResult> Create()
        {

            var accessToken = await HttpContext.GetTokenAsync("MVCIdentityScheme", "access_token");
            _resourceApi1.SetBearerToken(accessToken);

            var result = await _resourceApi1.GetAsync(ResourceApi1EndPoint.InstructorCreateEndPoint);

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();

                return View();
            }

            return StatusCode(403);
        }


        public async Task<IActionResult> Deny()
        {

            var accessToken = await HttpContext.GetTokenAsync("MVCIdentityScheme", "access_token");
            _resourceApi1.SetBearerToken(accessToken);

            var result = await _resourceApi1.GetAsync(ResourceApi1EndPoint.InstructorDenyEndPoint);

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();

                return View();
            }

            return StatusCode(403);
        }
    }
}
