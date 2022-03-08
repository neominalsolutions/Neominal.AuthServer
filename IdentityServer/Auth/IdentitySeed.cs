using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Auth
{
    public static class IdentitySeed
    {
        public static void Seed(ConfigurationDbContext configurationDbContext)
        {
            if (!configurationDbContext.Clients.Any())
            {
                foreach (var item in AuthConfig.GetClients())
                {
                    configurationDbContext.AddRange(item.ToEntity());
                }

               
            }

            if (!configurationDbContext.ApiResources.Any())
            {
                foreach (var item in AuthConfig.GetApiResources())
                {
                    configurationDbContext.AddRange(item.ToEntity());
                }

                
            }


            if (!configurationDbContext.ApiScopes.Any())
            {
                foreach (var item in AuthConfig.GetApiScopes())
                {
                    configurationDbContext.AddRange(item.ToEntity());
                }

                
            }

            if (!configurationDbContext.IdentityResources.Any())
            {
                foreach (var item in AuthConfig.GetIdentityResources())
                {
                    configurationDbContext.AddRange(item.ToEntity());
                }

                
            }

            

            configurationDbContext.SaveChanges();
        }
    }
}
