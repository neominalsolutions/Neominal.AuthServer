﻿using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Auth
{
    // POST isteği Body Token introspection endpoint request
    // 200 dönerse token active yada pasif mi bilgilerini verir yetkimiz yoksa 401 döndürür.
    // https://developer.okta.com/blog/2017/07/25/oidc-primer-part-1 profile claims

    // iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/main/getmain.ps1'))
    // yukarıdaki kodu powershell ile çalıştırarak otomatik olarak templatelere sahip oluyoruz.


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
            return new List<Client> {

                new Client {
                ClientName = "client1 app",
                ClientId = "client1",
                ClientSecrets = new[] {
                    new Secret("x-secret".Sha256()) // şifrelenmiş HASH değeri HASH değeri ile kıyaslayacağız 
                },
                AllowedGrantTypes = GrantTypes.ClientCredentials, // Üyelik mekanizması yok
                AllowedScopes = { "api1.read" } // api üzerinden hangi scope erişmeye izinlidir.
                // scope tanımlaması yapınca identiy server otomatik olarak bu scopeların hangi api izinli olduğunu biliyor.
                },
                new Client
                {
                    RequirePkce = false, // serverside uygulamalarda bu özelliği merkezi üyelik sistemi için yönetmemize gerek yoktur
                    ClientName = "MvcClient1 App",
                    ClientId="MvcClient1",
                    ClientSecrets =  new[] {
                    new Secret("x-secret".Sha256()) // şifrelenmiş HASH değeri HASH değeri ile kıyaslayacağız 
                    },
                    AllowedGrantTypes = GrantTypes.Hybrid, // code id_token istedeiğimiz için
                    AllowedScopes = {IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile 
                    }, // resource owner hangi bilgilerine ulşamak istiyoruz
                    RedirectUris = new List<string>{ "https://localhost:5004/signin-oidc" }
                    // openId connect ile authenticate işlemi sonrasında client da ilgili sayfaya yönlenecek ve code id_token bilgilerini alabileceğiz.
                }

            };
        }

        /// <summary>
        /// Token içerisinde user ile ilgili hangi dataları tutacağımızı ayalarız. Identity ait resourceslar.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
                new  IdentityResources.OpenId(), // Tokenın hangi kullanıcı tarafından tüketildiğine dair subject Id bilgisi tutar. Üyelik mekanizması devreye girdiği andan itibareten üretilen access token hangi kimliğe sahip olduğununu bu OpenId sayesinde uniqueleştiririz.Bu alanın gönderilmesi zorunludur. Token içerisinde tutulacak SubId yani SubjectId karşılık gelir.
                // required scope
                new IdentityResources.Profile(), // Kullanıcı isim soyisim vs kullanıcıya ait profil bilgilerini alacağız. Idnetity serverdan gelen Ön tanımlı resource'lar.

                };
        }

        /// <summary>
        /// Uygulamamızı kullancak test user bilgileri
        /// </summary>
        /// <returns></returns>
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
                        new Claim("email","test@test.com")
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
                        new Claim("email","cagatay@test.com")
                    }
                }
            };
        }
    }
}
