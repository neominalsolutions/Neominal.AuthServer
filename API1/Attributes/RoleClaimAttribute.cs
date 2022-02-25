using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1.Attributes
{
    // Bu yöntemde issued claims jwt'den gönderilmeli
    public class RoleClaimAttribute: ActionFilterAttribute
    {
        private string _claimType;
        private string _claimValue;
        private IHttpContextAccessor _httpContextAccessor;

        public RoleClaimAttribute(string claimType, string claimValue, IHttpContextAccessor httpContextAccessor)
        {
            _claimType = claimType;
            _claimValue = claimValue;
            _httpContextAccessor = httpContextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var authResult = _httpContextAccessor.HttpContext.AuthenticateAsync().GetAwaiter().GetResult();



            if (authResult.Succeeded)
            {
                var claim = authResult.Principal.Claims.FirstOrDefault(x => x.Type == this._claimType && x.Value == this._claimValue);

                if (claim == null)
                {                    
                    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                }
            }

            base.OnActionExecuting(context);
        }

       
    }
}
