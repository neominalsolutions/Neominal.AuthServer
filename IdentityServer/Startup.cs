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
        //     "jwks_uri": "https://localhost:5001/.well-known/openid-configuration/jwks", api identity server üzerinden public key öðrenirler

        //  "check_session_iframe": "https://localhost:5001/connect/checksession", kullanýcýnýn sesion durumunu kontrol eder

        //   "end_session_endpoint": "https://localhost:5001/connect/endsession", kullanýcý sesion sonlandýrýr.

        // "token_endpoint": "https://localhost:5001/connect/token", token endpoint

        //    "authorization_endpoint": "https://localhost:5001/connect/authorize", yetkilendirme end point

        // https://localhost:5001/connect/userinfo oturum açan kullanýcýya ait bilgiler

        // "revocation_endpoint": "https://localhost:5001/connect/revocation", refresh token geçersiz kýlma

        //  "introspection_endpoint": "https://localhost:5001/connect/introspect", token library kullanmadan parse etmek istersek kullanacaðýmýz endpoint
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
                //.AddSigningCredential() // production çýkarken ise bu Credential kullanacaðýz. Azure Amazon vs üzerinde tutulabilir.
                // uygulamayý nerede host ediyorsak bu bilgileri oradan almasý gerekiyor.
                .AddDeveloperSigningCredential(); // Development ortamýnda kullanacaðým public key ve private key oluþturur. Uygulama derlenince oluþacak olan bir key

            // Not: JSON web token kullanýrken Access Token oluþturmak için 2 farklý þifreleme yöntemi kullanýlýr. Simetrik þifreleme ve Asimetrik þifreleme. Simetrik þifrelemede access token oluþtururken ve doðrularken ayný þifreleme algoritmasýný kullanýyoruz. Asimetrik þifrelemede ise farklý þifreleme algoritmasý kullanýyoruz.
            // Asimetrik þifreleme algoritmasýnda private key ve public key vardýr. Yani 2 tane þifreleme anahtarý vardýr. Private key kimse ile paylaþýlmaz. Public key þifreyi kim çözecek ise onunla payþlaþýlýr. Public key sahip bir kiþi private key ile gelen datayý doðrulayabilir. Identity Server JSON Web tokenlarý imzalamak için asimetrik þifreleme kullanýr.
            // Auth server ilgili token daðýtýmdan önce private key ile þifreler.
            // Private key kendi içindedir kimse ile paylaþmaz
            // Clietlar private key ile þifrelenmiþ olan bu JSON Web tokenlerý Api gönderdiðinde API bu private tokenlarý public key ile doðrular.
            // buradaki yapýyý kilit anahtar iliþkisi gibi düþünebiliriz.
            // Bu apiler public key auth server üzerindeki bir endpoint vasýtasý ile alýrlar.
            // Auth server üzerinden bu public key kötü niyetli bir kullanýcýda öðrenebilir.ç Herkese açýk bir keydir bu.
            // Api üzerinde bu publickey access tokendan gönderilen private key ile doðrulanýr. Bu sayede bu daðýtýmýn Auth server üzerinden yapýldýðýný api anlar.




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
            app.UseIdentityServer(); // Identity server sürecini pipeline dahil et.
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
