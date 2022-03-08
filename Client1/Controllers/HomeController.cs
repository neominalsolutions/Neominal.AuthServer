using Client1.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _identityServer;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _identityServer = httpClientFactory.CreateClient("IdentityServer");
            _logger = logger;
            _configuration = configuration;
        }

        [Authorize(AuthenticationSchemes = "ClientCredentialsScheme")]
        public async Task<IActionResult> GetClientCredentialToken()
        {
            var result = await HttpContext.AuthenticateAsync("ClientCredentialsScheme");
            var accessToken = await HttpContext.GetTokenAsync("ClientCredentialsScheme", "access_token");



            return View();

        }


        [Authorize(AuthenticationSchemes = "IdentityServerScheme")]
        public async Task<IActionResult> Index()
        {
            var result = await HttpContext.AuthenticateAsync("IdentityServerScheme");

            var model = new AuthenticationViewModel
            {
                AuthenticatedUserId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value,
                Claims = User.Claims.ToList(),
                Properties = result.Properties.Items
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ChangeRefreshToken()
        {

            var discovery = await _identityServer.GetDiscoveryDocumentAsync(_identityServer.BaseAddress.AbsoluteUri);

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var requestToken = new RefreshTokenRequest();
            requestToken.Address = discovery.TokenEndpoint;
            requestToken.ClientId = _configuration["Client1:ClientId"];
            requestToken.ClientSecret = _configuration["Client1:ClientSecret"];
            requestToken.RefreshToken = refreshToken;

            var tokenResult = await _identityServer.RequestRefreshTokenAsync(requestToken);

            if (tokenResult.IsError)
            {
                throw new Exception("Refresh Token Error");
            }

            // identity serverdan gelen id_token, access_token, refresh_token bilgilerini StoreTokens ile HttpContext tanıttık.
            var tokens = new List<AuthenticationToken>
            {
                new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = tokenResult.IdentityToken },
                new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = tokenResult.RefreshToken },
                new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = tokenResult.AccessToken }, new AuthenticationToken { Name = OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.UtcNow.AddSeconds(tokenResult.ExpiresIn).ToString("O") },
            };

            var authResult = await HttpContext.AuthenticateAsync();

            var properties = authResult.Properties;

            properties.StoreTokens(tokens);
            await HttpContext.SignInAsync("MVCIdentityScheme", authResult.Principal, properties);

            return Redirect("/users");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("MVCIdentityScheme"); 
            await HttpContext.SignOutAsync("IdentityServerScheme"); 
        }
    }
}
