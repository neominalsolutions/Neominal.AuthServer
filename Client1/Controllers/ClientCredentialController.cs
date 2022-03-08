using Client1.Models;
using Client1.Utils.EndPoints;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client1.Controllers
{
    public class ClientCredentialController : Controller
    {
        private readonly HttpClient _identityServer;
        private readonly HttpClient _resourceApi1;
        private readonly IConfiguration _configuration;

        public ClientCredentialController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _identityServer = httpClientFactory.CreateClient("IdentityServer");
            _resourceApi1 = httpClientFactory.CreateClient("ResourceApi1");
            _configuration = configuration;
        }


        [Authorize(AuthenticationSchemes = "ClientCredentialsScheme")]
        public async Task<IActionResult> CallApi()
        {
           
            var accessToken = await HttpContext.GetTokenAsync("ClientCredentialsScheme", "access_token");
            _resourceApi1.SetBearerToken(accessToken);
            var result = await _resourceApi1.GetAsync(ResourceApi1EndPoint.WeatherForecastUrl);

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<List<WeatherForecastDto>>(response);

                return View(data);
            }

            return Unauthorized();

        }



        public async Task<IActionResult> Index()
        {


            var discovery = await _identityServer.GetDiscoveryDocumentAsync(_identityServer.BaseAddress.AbsoluteUri);


            var requestToken = new ClientCredentialsTokenRequest();
            requestToken.Address = discovery.TokenEndpoint;
            requestToken.ClientId = _configuration["ClientCredential:ClientId"];
            requestToken.ClientSecret = _configuration["ClientCredential:ClientSecret"];



            var accessTokenResult = await _identityServer.RequestClientCredentialsTokenAsync(requestToken);


            var tokens = new List<AuthenticationToken>
            {
               
                new AuthenticationToken { Name = "access_token", Value = accessTokenResult.AccessToken },
                new AuthenticationToken { Name = OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.UtcNow.AddSeconds(accessTokenResult.ExpiresIn).ToString("O") },
            };

            var claims = new List<Claim>
            {
               new Claim("ClientId",_configuration["ClientCredential:ClientId"])
            };

            var claimPrinciple = new ClaimsPrincipal();

            var identity = new ClaimsIdentity(claims, "ClientCredentialsScheme");
            claimPrinciple.AddIdentity(identity);


            var properties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.Now.AddDays(31),
                IsPersistent = true,
            };

            properties.StoreTokens(tokens);
            await HttpContext.SignInAsync("ClientCredentialsScheme", claimPrinciple, properties);

            return Redirect("/Home/GetClientCredentialToken");

  
        }
    }
}
