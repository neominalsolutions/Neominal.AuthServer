using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1.AuthorizationHandlers
{
    public class PermissionRequirement: IAuthorizationRequirement
    {
        public string ApiName { get; private set; }

        public PermissionRequirement(string apiName)
        {
            if (string.IsNullOrEmpty(apiName))
            {
                throw new Exception("ApiName boş geçilemez");
            }

            ApiName = apiName;
        }
    }
}
