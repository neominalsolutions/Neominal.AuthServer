using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API1.Attributes
{
    public class RoleClaimCustomerInfoAttribute: IAsyncActionFilter
    {
        private string _claimType;
        private string _claimValue;
        private readonly HttpClient _identityServer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleClaimCustomerInfoAttribute(string claimType,string claimValue,IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _claimType = claimType;
            _claimValue = claimValue;
            _identityServer = httpClientFactory.CreateClient("IdentityServer");
            _httpContextAccessor = httpContextAccessor;
        }

     

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var discovery = await _identityServer.GetDiscoveryDocumentAsync(_identityServer.BaseAddress.AbsoluteUri);

            var authResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);


            var response = await _identityServer.GetUserInfoAsync(new UserInfoRequest
            {
                Address = discovery.UserInfoEndpoint,
                Token = accessToken
            });


            if (authResult.Succeeded)
            {
                var claim = response.Claims.FirstOrDefault(x => x.Type == this._claimType && x.Value == this._claimValue);

                if (claim == null)
                {
                    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                    await Task.CompletedTask;
                    
                }
            }
            else
            {
                await next();
            }

            

        }
    }
}
