using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Auth
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // _userManager.GetClaimsAsync(user),
            // context.Subject.Identity.Name // authenticated user

            var Claims = new List<Claim>
                    {
                        new Claim("name","mert@test.com"),
                        new Claim("given_name","Mert Alptekin"),
                        new Claim("email","test@test.com"),
                        new Claim("country","türkiye"),
                        new Claim("city","istanbul"),
                        new Claim("role","admin"),
                        new Claim("ProductControllerRequest","ProductControllerRequest"),
                        new Claim("WeatherControllerRequest","WeatherControllerRequest")
                    };

            context.AddRequestedClaims(Claims);
            //context.IssuedClaims.AddRange(Claims); // accessToken içerisine claim göm

            return Task.FromResult(0);
        }

        public  Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(0);
        }
    }
}
