using Amazon;
using Amazon.S3;
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
using TelecomSTE.DE3.ResumeAnalyzer.Api.Business;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Business.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Configuration;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api
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
            services.AddCors(options =>
            {
                options.AddPolicy("localhost", builder =>
                {
                    builder.WithOrigins("http://jobpredict").AllowAnyHeader().AllowAnyMethod();
                });
            });
            //Injection de dépendance


            // Ajout de la configuration applicative
            var settings = new Settings();
            ConfigurationBinder.Bind(Configuration, settings);
            services.AddSingleton(settings);
            // --------

            //---- Ajout des services et repos
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IAnalysisResultRepository, AnalysisResultRepository>();
            services.AddTransient<IUpdateTimeRepository, UpdateTimeRepository>();
            services.AddTransient<IWordCountRepository, WordCountRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IAnalysisDataService, AnalysisDataService>();

            //----
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TelecomSTE.DE3.ResumeAnalyzer.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TelecomSTE.DE3.ResumeAnalyzer.Api v1"));
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options => options.WithOrigins("http://jobpredict").AllowAnyMethod());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
