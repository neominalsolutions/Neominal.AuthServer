using Client1.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client1.Controllers
{
    [Authorize(AuthenticationSchemes = "IdentityServerScheme")]
    public class UsersController : Controller
    {
        private readonly HttpClient _identityServer;
        private readonly IConfiguration _configuration;

        public UsersController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _identityServer = httpClientFactory.CreateClient("IdentityServer");
            _configuration = configuration;
        }
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


        /// <summary>
        /// Refresh token bilgisini gönderip yeni bir access token almamızı sağlar.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ChangeRefreshToken()
        {
            // identity server refresh token endpointi bulduk
            var discovery = await _identityServer.GetDiscoveryDocumentAsync(_identityServer.BaseAddress.AbsoluteUri);

            // refresh token HttpContext.GetTokenAsync ile alabiliriz.

            // cookiedeki refresh token bilgimizi okuduk
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var requestToken = new RefreshTokenRequest();
            requestToken.Address = discovery.TokenEndpoint;
            requestToken.ClientId = _configuration["Client1:ClientId"];
            requestToken.ClientSecret = _configuration["Client1:ClientSecret"];
            requestToken.RefreshToken = refreshToken;

            // yeni bir refresh token isteğinde bulunduk
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

            // ilgili şemaya yeniden signIn oluyoruz. değişen token bilgileri ile bu bilgiler zaten HttpContext properties üzerinden geliyor.
            // kullanıcıyı yeniden oturum açtırdık
            await HttpContext.SignInAsync("MVCIdentityScheme", authResult.Principal, properties);

            return Redirect("/users");

        }
    }
}
