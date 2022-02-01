using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client1.Services
{
    public static class HttpClientExtensions
    {
     
        public static async Task SetAccessToken(this HttpClient httpClient, HttpContext context)
        {
            var accessToken = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            // cookie üzerinden access token alalım

            httpClient.SetBearerToken(accessToken);
        }
    }

}
