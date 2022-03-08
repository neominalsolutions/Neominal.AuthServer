using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Auth
{
   
    public static class AuthConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> {
                new ApiResource("api1", "API 1") // api1 Basic Auth için api username
                {
                    Scopes = { "GET", "POST","PUT","DELETE" },
                    ApiSecrets = new[]{new Secret("secretapi1".Sha256()) },
                   
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope> {
                new ApiScope("GET", "PUT Access"),
                new ApiScope("POST", "POST Access"),
                  new ApiScope("PUT", "PUT Access"),
                   new ApiScope("DELETE", "DELETE Access")

            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client
                {
                    ClientName = "MVCClientCredential1 App",
                    ClientId = "MVCClientCredential",
                     ClientSecrets =  new[] {
                    new Secret("secret".Sha256())
                    },
                       AllowedGrantTypes = GrantTypes.ClientCredentials, 
                    AllowedScopes = {"GET"},
                },
                new Client
                {
                    RequirePkce = false, 
                    ClientName = "MvcClient1 App",
                    ClientId="MvcClient1",
                    ClientSecrets =  new[] {
                    new Secret("x-secret".Sha256()) 
                    },
                    PostLogoutRedirectUris = new List<string> { "https://localhost:5004/signout-callback-oidc" }, // Default bir redirect uri
                    AllowedGrantTypes = GrantTypes.Hybrid, // code id_token istedeiğimiz için
                    AllowedScopes = {IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile
                    , IdentityServerConstants.StandardScopes.OfflineAccess, "CountryAndCity","Roles","RoleClaims","GET"}, 
                    RedirectUris = new List<string>{ "https://localhost:5004/signin-oidc" },
                    AllowOfflineAccess = true, 
                   AccessTokenLifetime =(int)(DateTime.Now.AddHours(2)- DateTime.Now).TotalSeconds, // Default 1 saat
                   RefreshTokenUsage = TokenUsage.ReUse, 
                   RefreshTokenExpiration = TokenExpiration.Absolute, 
                   AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(15) - DateTime.Now).TotalSeconds,  
                   AlwaysSendClientClaims = true,
                   
                   AlwaysIncludeUserClaimsInIdToken = true
                },
                // new Client
                //{
                //    ClientName = "Angular SPA Client1",
                //    ClientId="AngularClient1",
                //    RequireClientSecret = false, // JS uygulaması olduğu için
                //    PostLogoutRedirectUris = { "http://localhost:4200" },
                //    AllowedGrantTypes = GrantTypes.Hybrid, // Authorization Code akışı
                //    // client'ın hem api hemde oturum açan kullanıcı ile ilgili erişebileceği scopeları
                //    AllowedScopes = {IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile
                //    ,"GET", IdentityServerConstants.StandardScopes.OfflineAccess, "CountryAndCity","Roles"},
                //    RedirectUris = new List<string>{ "http://localhost:4200/callback" },
                //    AllowedCorsOrigins = { "http://localhost:4200" },
                //    AllowOfflineAccess = true,
                //   AccessTokenLifetime = 3600,
                //   RefreshTokenUsage = TokenUsage.ReUse,
                //   RefreshTokenExpiration = TokenExpiration.Absolute,
                //   AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(15) - DateTime.Now).TotalSeconds,
                //   RequireConsent = false
                //}

            };
        }

       
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
                new  IdentityResources.OpenId(), 
                new IdentityResources.Profile(), 
                new IdentityResource(){ Name="CountryAndCity", Description="şehir ve ülke bilgisi", DisplayName="CountryAndCity", UserClaims = {"country","city" } },
                new IdentityResource() {Name  ="Roles", Description="User Role", UserClaims= {"role"}
                },
                new IdentityResource() {Name= "RoleClaims", Description = "Role Claims", UserClaims = { "ProductControllerRequest", "WeatherControllerRequest" }
                }
                // identity resource içerisinde tüm RoleClaimsleri dışarı çıkarmamız lazım. Ne kadar sistemde permission varsa buraya atacağız
            };

        }


        public static IEnumerable<TestUser> GetUsers()
        {
            return new List<TestUser>{
                new TestUser
                {
                    Password = "password1",
                    Username = "mert.alptekin",
                    SubjectId = "dbb05323-f54d-477e-a243-50a7be8ca109",
                    Claims = new List<Claim>
                    {
                        new Claim("name","mert@test.com"),
                        new Claim("given_name","Mert Alptekin"),
                        new Claim("email","test@test.com"),
                        new Claim("country","türkiye"),
                        new Claim("city","istanbul"),
                        new Claim("role","admin"),
                        new Claim("ProductControllerRequest","ProductControllerRequest"),
                        new Claim("WeatherControllerRequest","WeatherControllerRequest")
                    }
                },
                 new TestUser
                {
                    Password = "password1",
                    Username = "cagatay.yildiz",
                    SubjectId = "215a3220-65cb-43fd-8c9a-c18743b6fe37",
                    Claims = new List<Claim>
                    {
                        new Claim("name","cagatay@test.com"),
                        new Claim("given_name","Çağatay Yıldız"),
                        new Claim("email","cagatay@test.com"),
                        new Claim("ProductControllerRequest","ProductControllerRequest")
                    }
                }
            };
        }
    }
}
