using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            services.AddControllersWithViews();
            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = "MVCIdentityScheme"; // Uygulamaya Indentity Scheme dýþýnda default bir authentication þemasý veriyoruz.
                opt.DefaultChallengeScheme = "IdentityServerScheme"; // IdentityServerScheme ile Identity þemasý üzerinden authenticate olduk.

            }).AddCookie("MVCIdentityScheme").AddOpenIdConnect("IdentityServerScheme", opt =>
            {

                opt.SignInScheme = "MVCIdentityScheme"; // Identity server üzerinden kimliði doðrulanan kullanýcýnýn cookie bilgisi
                opt.Authority = Configuration["ApiUrls:IdentityServer"];
                opt.ClientId = "MvcClient1"; // Identity serverde client app için tanýmladýðýmýz isim
                opt.ClientSecret = "x-secret"; // Identity serverde client için tanýmladýðýmýz key
                opt.ResponseType = "code id_token"; // Identity serverdan istencek response type, authorization code va identity token doðru bir saðlayýcýdan bu kimlik bilgilerini aldýðýmýza dair ayar için kullanýyoruz. hibrit bir akýþ sunuyor.

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
