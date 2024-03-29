using IdentityServer.Auth;
using IdentityServer.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Startup
    {
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            var assemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;




            services.AddDbContext<EFIdentityDbContext>(opt => {
                opt.UseSqlServer(Configuration.GetConnectionString("IdentityDbConn"));
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<EFIdentityDbContext>().AddDefaultTokenProviders();

            services.AddIdentityServer()
                
                .AddConfigurationStore(opt =>
                {
                    opt.ConfigureDbContext = c => c.UseSqlServer(Configuration.GetConnectionString("IdentityDbConn"), sqlOptions => sqlOptions.MigrationsAssembly(assemblyName));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = c => c.UseSqlServer(Configuration.GetConnectionString("IdentityDbConn"), sqlOptions => sqlOptions.MigrationsAssembly(assemblyName));
                })

                //.AddInMemoryApiResources(AuthConfig.GetApiResources())
                //.AddInMemoryApiScopes(AuthConfig.GetApiScopes())
                //.AddInMemoryClients(AuthConfig.GetClients())
                //.AddInMemoryIdentityResources(AuthConfig.GetIdentityResources())
                //.AddTestUsers(AuthConfig.GetUsers().ToList())
                //.AddProfileService<ProfileService>()
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential(); 

          

            services.AddControllersWithViews();
            //services.AddLocalApiAuthentication(); //signup i�lemleri i�in apiden istek almak i�in kulland�k
            // farkl� bir resource api i�in bir deniyelim
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
