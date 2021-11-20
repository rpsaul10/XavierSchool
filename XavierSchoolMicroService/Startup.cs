using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Bussiness;

namespace XavierSchoolMicroService
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
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "XavierSchoolMicroService", Version = "v1" });
            });

            services.AddDbContext<XavierSchoolMicroService.Models.escuela_xavierContext>(options => {
                var connectionString = Configuration.GetConnectionString("XavierSchoolConnectionString");
                options.UseMySQL(connectionString);
            });

             services.AddCors(options =>
            {
               options.AddPolicy ("MY_CORS", builder =>
               {
                   builder.WithOrigins ("http://localhost:3000");
                   builder.AllowAnyMethod ();
                   builder.AllowAnyHeader ();
               });
            });

            // Puede liquidar
            services.AddDataProtection()
                .SetApplicationName("XavierSchoolMicroService")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(14))
                .UseCryptographicAlgorithms(
                    new AuthenticatedEncryptorConfiguration
                    {
                        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                    }
                );

            services.AddScoped<IServiceEstudiante, ServiceEstudiante>();
            services.AddScoped<IServiceProfesores, ServiceProfesores>();
            services.AddScoped<IServiceLecPublicas, ServiceLecPublicas>();
            services.AddScoped<IServiceLecPrivadas, ServiceLecPrivadas>();
            services.AddScoped<IServicePresentaciones, ServicePresentaciones>();
            services.AddScoped<IServicePoderes, ServicePoderes>();
            services.AddScoped<IServiceUsuarios, ServiceUsuarios>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("MY_CORS");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.jason", "XavierSchoolMicroService");
            });
        }
    }
}
