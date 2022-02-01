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
                opt.DefaultScheme = "MVCIdentityScheme"; // Uygulamaya Indentity Scheme d���nda default bir authentication �emas� veriyoruz.
                opt.DefaultChallengeScheme = "IdentityServerScheme"; // IdentityServerScheme ile Identity �emas� �zerinden authenticate olduk.

            }).AddCookie("MVCIdentityScheme").AddOpenIdConnect("IdentityServerScheme", opt =>
            {

                opt.SignInScheme = "MVCIdentityScheme"; // Identity server �zerinden kimli�i do�rulanan kullan�c�n�n cookie bilgisi
                opt.Authority = Configuration["ApiUrls:IdentityServer"];
                opt.ClientId = "MvcClient1"; // Identity serverde client app i�in tan�mlad���m�z isim
                opt.ClientSecret = "x-secret"; // Identity serverde client i�in tan�mlad���m�z key
                opt.ResponseType = "code id_token"; // Identity serverdan istencek response type, authorization code va identity token do�ru bir sa�lay�c�dan bu kimlik bilgilerini ald���m�za dair ayar i�in kullan�yoruz. hibrit bir ak�� sunuyor.
                opt.GetClaimsFromUserInfoEndpoint = true; // otomatik olarak user-profile ile ilgili claim i�erisibe g�nm�� olduk. Yoksa UserInfoEndpoint �zerinden Access token g�nderirerek Identity Server �zerinden user profile bilgilerine ula�abiliriz.
                opt.SaveTokens = true; // uygulama scope access token ve refresh tokenlar� kaydetmek istersek bu �zelli�i true yapar�z.
                // Default false olarak ayarlanm��t�r.
                opt.Scope.Add("api1.read"); // api read izni ver. Client i�in tan�mlanm�� yetkileri identity serverde allowedscope verirken buradan da client hangi izinlere (yetkilere) sahip olamas� istede�ini se�iyoruz.
                opt.Scope.Add("offline_access"); // Refresh Token iste�ini aktif hale getirdik.
                opt.Scope.Add("CountryAndCity");
                opt.Scope.Add("Roles"); // buraya Identity Resource ekliyoruz. User Claim de�il (userclaim :role)
                // custom claim olu�turma i�lemi
                // custom claimleri maplememiz laz�m
                // custom Resource Identity Tan�m�
                opt.ClaimActions.MapUniqueJsonKey("country", "country");
                opt.ClaimActions.MapUniqueJsonKey("city", "city");
                opt.ClaimActions.MapUniqueJsonKey("role", "role"); // burada ise Identity Resource �zerinden user claim veriyoruz.


                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "role" // user-claim veriyoruz
                };


            });
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
