using Client1.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client1
{
    public class Startup
    {
        // IdentityModel Package OpenId Connect ve Auth2.0 client library

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllersWithViews();
            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = "MVCIdentityScheme"; // Uygulamaya Indentity Scheme dýþýnda default bir authentication þemasý veriyoruz.
                opt.DefaultChallengeScheme = "IdentityServerScheme"; // IdentityServerScheme ile Identity þemasý üzerinden authenticate olduk.

            }).AddCookie("MVCIdentityScheme").AddOpenIdConnect("IdentityServerScheme", opt =>
            {

                opt.SignInScheme = "MVCIdentityScheme";
                opt.Authority = Configuration["ApiUrls:IdentityServer"];
                opt.ClientId = "MvcClient1";
                opt.ClientSecret = "x-secret";
                opt.ResponseType = "code id_token";
                opt.SaveTokens = true;

                opt.Scope.Add("GET");
                opt.Scope.Add("offline_access");
                opt.Scope.Add("CountryAndCity");
                opt.Scope.Add("Roles");
                opt.Scope.Add("RoleClaims");
                // role dýþýnda 
                //opt.ClaimActions.MapJsonKey("permission", "Api1.Products.GetProducts");
                //opt.ClaimActions.MapJsonKey("WeatherControllerRequest", "WeatherControllerRequest");
                //opt.ClaimActions.MapUniqueJsonKey("country", "country");
                //opt.ClaimActions.MapUniqueJsonKey("city", "city");
                //opt.ClaimActions.MapUniqueJsonKey("role", "role");

                //opt.ClaimActions.MapUniqueJsonKey("permission", "permission");


                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "role",
                    NameClaimType = "name"
                   
                };


            }).AddCookie("ClientCredentialsScheme").AddOpenIdConnect(opt =>
            {

                opt.SignInScheme = "ClientCredentialsScheme";
                opt.Authority = Configuration["ApiUrls:IdentityServer"];
                opt.ClientId = "MVCClientCredential";
                opt.ClientSecret = "secret";
                opt.ResponseType = "code token";
                opt.SaveTokens = true;


            }); ;




            





            services.AddHttpClient("IdentityServer", opt => {

                opt.BaseAddress = new Uri(Configuration["ApiUrls:IdentityServer"]);
                opt.DefaultRequestHeaders.Add("User-Agent", "Client1");
            });

            services.AddHttpClient("ResourceApi1", opt =>
            {
                opt.BaseAddress = new Uri(Configuration["ApiUrls:ResourceApi1"]);
                opt.DefaultRequestHeaders.Add("User-Agent", "Client1");
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

    

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
