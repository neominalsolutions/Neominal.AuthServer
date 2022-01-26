using IdentityServer.Auth;
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

namespace IdentityServer
{
    public class Startup
    {
        //     "jwks_uri": "https://localhost:5001/.well-known/openid-configuration/jwks", api identity server �zerinden public key ��renirler

        //  "check_session_iframe": "https://localhost:5001/connect/checksession", kullan�c�n�n sesion durumunu kontrol eder

        //   "end_session_endpoint": "https://localhost:5001/connect/endsession", kullan�c� sesion sonland�r�r.

        // "token_endpoint": "https://localhost:5001/connect/token", token endpoint

        //    "authorization_endpoint": "https://localhost:5001/connect/authorize", yetkilendirme end point

        // https://localhost:5001/connect/userinfo oturum a�an kullan�c�ya ait bilgiler

        // "revocation_endpoint": "https://localhost:5001/connect/revocation", refresh token ge�ersiz k�lma

        //  "introspection_endpoint": "https://localhost:5001/connect/introspect", token library kullanmadan parse etmek istersek kullanaca��m�z endpoint
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddInMemoryApiResources(AuthConfig.GetApiResources())
                .AddInMemoryApiScopes(AuthConfig.GetApiScopes())
                .AddInMemoryClients(AuthConfig.GetClients())
                .AddInMemoryIdentityResources(AuthConfig.GetIdentityResources())
                .AddTestUsers(AuthConfig.GetUsers().ToList())
                //.AddSigningCredential() // production ��karken ise bu Credential kullanaca��z. Azure Amazon vs �zerinde tutulabilir.
                // uygulamay� nerede host ediyorsak bu bilgileri oradan almas� gerekiyor.
                .AddDeveloperSigningCredential(); // Development ortam�nda kullanaca��m public key ve private key olu�turur. Uygulama derlenince olu�acak olan bir key

            // Not: JSON web token kullan�rken Access Token olu�turmak i�in 2 farkl� �ifreleme y�ntemi kullan�l�r. Simetrik �ifreleme ve Asimetrik �ifreleme. Simetrik �ifrelemede access token olu�tururken ve do�rularken ayn� �ifreleme algoritmas�n� kullan�yoruz. Asimetrik �ifrelemede ise farkl� �ifreleme algoritmas� kullan�yoruz.
            // Asimetrik �ifreleme algoritmas�nda private key ve public key vard�r. Yani 2 tane �ifreleme anahtar� vard�r. Private key kimse ile payla��lmaz. Public key �ifreyi kim ��zecek ise onunla pay�la��l�r. Public key sahip bir ki�i private key ile gelen datay� do�rulayabilir. Identity Server JSON Web tokenlar� imzalamak i�in asimetrik �ifreleme kullan�r.
            // Auth server ilgili token da��t�mdan �nce private key ile �ifreler.
            // Private key kendi i�indedir kimse ile payla�maz
            // Clietlar private key ile �ifrelenmi� olan bu JSON Web tokenler� Api g�nderdi�inde API bu private tokenlar� public key ile do�rular.
            // buradaki yap�y� kilit anahtar ili�kisi gibi d���nebiliriz.
            // Bu apiler public key auth server �zerindeki bir endpoint vas�tas� ile al�rlar.
            // Auth server �zerinden bu public key k�t� niyetli bir kullan�c�da ��renebilir.� Herkese a��k bir keydir bu.
            // Api �zerinde bu publickey access tokendan g�nderilen private key ile do�rulan�r. Bu sayede bu da��t�m�n Auth server �zerinden yap�ld���n� api anlar.




            services.AddControllersWithViews();
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
            app.UseIdentityServer(); // Identity server s�recini pipeline dahil et.
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
