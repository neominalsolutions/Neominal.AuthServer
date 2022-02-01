
using Client1.Models;
using Client1.Services;
using Client1.Utils.EndPoints;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient _identityServer;
        private readonly HttpClient _resourceApi1;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductsController> _logger;
        private readonly IHttpClientInterceptor _resourceApi1Interceptor;
        public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ProductsController> logger, IHttpClientInterceptor resouceApi1Interceptor)
        {
            _identityServer = httpClientFactory.CreateClient("IdentityServer");
            _resourceApi1 = httpClientFactory.CreateClient("ResourceApi1");
            _configuration = configuration;
            _logger = logger;
            _resourceApi1Interceptor = resouceApi1Interceptor;
        }

      

        public async Task<IActionResult> Index()
        {
            // Identity Model httpclient extension sayesinde Discovery Endpoint kullanarak tüm identity server urileri çektik.
           var discovery =  await _identityServer.GetDiscoveryDocumentAsync(_identityServer.BaseAddress.AbsoluteUri);
            //https://localhost:5001/.well-known/openid-configuration
            //  https://localhost/5001/.well-known/openid-configuration


            if (discovery.IsError) // Eğer hata varsa
            {
                throw new Exception(discovery.Error);
            }

            // client credential bilgileri ile token almaya çalışıyorum.
            var tokenRequest = new ClientCredentialsTokenRequest();
            tokenRequest.ClientId = _configuration["Client:ClientId"];
            tokenRequest.ClientSecret = _configuration["Client:ClientSecret"];
            tokenRequest.Address = discovery.TokenEndpoint;

           var tokenResponse  =  await _identityServer.RequestClientCredentialsTokenAsync(tokenRequest);

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            // accessToken ve RefreshToken bilgisine sahip olabiliriz.

            // IdentityModel üzerinden Basic Authentication veya Bearer Authentication yapabiliriz.
            _resourceApi1.SetBearerToken(tokenResponse.AccessToken);
            // ilgili isteğin header'ına ekleriz.

            var response = await _resourceApi1.GetAsync(ResourceApi1EndPoint.ProductUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var model = System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(jsonString, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true }); ;

                return View(model);
            }
            else
            {
                _logger.LogError("ResouceApi1 not connected");
            }


            return View();
        }

        public async Task<IActionResult> List()
        {
            // httpContext üzerindeki Access Token bilgisini okuyup. bu access token bilgisini client set ettik. Extension ile
            await _resourceApi1.SetAccessToken(HttpContext);
            // http context üzerinden access token set etmek için kullandık.
            var response =  await _resourceApi1.GetAsync(ResourceApi1EndPoint.ProductUrl);
            // products endpointe istek attık

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                // isteği json formatında okuduk
                var model = System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(jsonString, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true }); ;

                return View(model);
            }
            

            return Unauthorized();

        }
    }
}
