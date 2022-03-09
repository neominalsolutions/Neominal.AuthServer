using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API1.AuthorizationHandlers
{
    public class PermissonRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _identityServer;


        public PermissonRequirementHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _identityServer = httpClientFactory.CreateClient("IdentityServer");
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            var discovery = await _identityServer.GetDiscoveryDocumentAsync(_identityServer.BaseAddress.AbsoluteUri);

            var authResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);


            var response = await _identityServer.GetUserInfoAsync(new UserInfoRequest
            {
                Address = discovery.UserInfoEndpoint,
                Token = accessToken
            });


            string controllerName = _httpContextAccessor.HttpContext.Request.RouteValues["controller"].ToString();
            string actionName = _httpContextAccessor.HttpContext.Request.RouteValues["action"].ToString();


            var claim = response.Claims.FirstOrDefault(x => x.Value == $"{requirement.ApiName}.{controllerName}.{actionName}");

            if (claim == null)
            {
                context.Fail();

            }
            else
            {
                context.Succeed(requirement);
            }


        }
    }
}
