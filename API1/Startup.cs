using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API1", Version = "v1" });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.Authority = "https://localhost:5001/"; 
                opt.Audience = "api1";
                opt.SaveToken = true;
            });

            services.AddHttpClient("IdentityServer", opt => {

                opt.BaseAddress = new Uri(Configuration["ApiUrls:IdentityServer"]);
                opt.DefaultRequestHeaders.Add("User-Agent", "ApiClient");
            });





            services.AddAuthorization(options => options.AddPolicy("CountryClaim", policy => policy.RequireClaim("country", "türkiye")));
            services.AddAuthorization(options => options.AddPolicy("GETACCESS", policy => policy.RequireClaim("scope", "GET")));


        }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API1 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
