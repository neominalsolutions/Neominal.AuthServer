using IdentityServer.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
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

      

            services.AddIdentityServer()
                .AddConfigurationStore(opt => {
                    opt.ConfigureDbContext = c => c.UseSqlServer(Configuration.GetConnectionString("IdentityDb"),sqlOptions => sqlOptions.MigrationsAssembly(assemblyName));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = c => c.UseSqlServer(Configuration.GetConnectionString("IdentityDb"), sqlOptions => sqlOptions.MigrationsAssembly(assemblyName));
                })
                //.AddInMemoryApiResources(AuthConfig.GetApiResources())
                //.AddInMemoryApiScopes(AuthConfig.GetApiScopes())
                //.AddInMemoryClients(AuthConfig.GetClients())
                //.AddInMemoryIdentityResources(AuthConfig.GetIdentityResources())
                .AddTestUsers(AuthConfig.GetUsers().ToList())
                .AddProfileService<ProfileService>()

                .AddDeveloperSigningCredential(); 

          

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
