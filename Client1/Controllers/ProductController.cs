using Client1.Models;
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
    public class ProductController : Controller
    {
        private readonly HttpClient _identityServer;
        private readonly HttpClient _resourceApi1;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _identityServer = httpClientFactory.CreateClient("IdentityServer");
            _resourceApi1 = httpClientFactory.CreateClient("ResourceApi1");
        }

        public async Task<IActionResult> List()
        {

            var accessToken = await HttpContext.GetTokenAsync("MVCIdentityScheme", "access_token");
            _resourceApi1.SetBearerToken(accessToken);

            var result =  await _resourceApi1.GetAsync(ResourceApi1EndPoint.ProductUrl);

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<List<ProductViewModel>>(response);

                return View(data);
            }

            return View();
        }
    }
}
