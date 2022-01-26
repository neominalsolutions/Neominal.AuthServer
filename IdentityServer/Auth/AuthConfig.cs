using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Auth
{
    // POST isteği Body Token introspection endpoint request
    // 200 dönerse token active yada pasif mi bilgilerini verir yetkimiz yoksa 401 döndürür.

    public static class AuthConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> {
                new ApiResource("api1", "API 1") // api1 Basic Auth için api username
                {
                    Scopes = { "api1.read" }, 
                    ApiSecrets = new[]{new Secret("secretapi1".Sha256()) } // api secrets ise Basic Auth için password karşılık geliyor. introspection endpoint ile 
                    // elimizdeki token ilgili api için aktif mi değil mi kontrolerini yapacağız. ilgili token ilgili api için yetkisi var mı yok mu kontrolü yaparız.
                    // Basic Authorization client ile server arasında kimlik doğrulama yapmak için kullanılan eski bir kimlik doğrulama şekliydi. Auth2.0 protokolü öncesi yaygın bir şekilde kullanılıyordu.
                  
                } 
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope> { new ApiScope("api1.read", "api1 get izni") };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> { new Client() {
                ClientName = "client1 app",
                ClientId = "client1",
                ClientSecrets = new[] {
                    new Secret("x-secret".Sha256()) // şifrelenmiş HASH değeri HASH değeri ile kıyaslayacağız 
                },
                AllowedGrantTypes = GrantTypes.ClientCredentials, // Üyelik mekanizması yok
                AllowedScopes = { "api1.read" } // api üzerinden hangi scope erişmeye izinlidir.
                // scope tanımlaması yapınca identiy server otomatik olarak bu scopeların hangi api izinli olduğunu biliyor.
            } };
        }

    }
}
