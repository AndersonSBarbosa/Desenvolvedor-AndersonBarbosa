using System;
using ProcurandoApartamento.Infrastructure.Data;
using ProcurandoApartamento.Configuration;
using ProcurandoApartamento.Infrastructure.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;

[assembly: ApiController]

namespace ProcurandoApartamento
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }

        public IHostEnvironment Environment { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAppSettingsModule(Configuration);

            AddDatabase(services);


            services
                .AddSecurityModule()
                .AddProblemDetailsModule(Environment)
                .AddAutoMapperModule()
                .AddSwaggerModule()
                .AddWebModule()
                .AddRepositoryModule()
                .AddServiceModule();


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Busca Apartamentos", Version = "v1" });
            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider,
            ApplicationDatabaseContext context, IOptions<SecuritySettings> securitySettingsOptions)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Busca Apartamentos v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var securitySettings = securitySettingsOptions.Value;
            app
                .UseApplicationSecurity(securitySettings)
                .UseApplicationProblemDetails()
                .UseApplicationSwagger()
                .UseApplicationWeb(env)
                .UseApplicationDatabase(serviceProvider, env);
        }

        protected virtual void AddDatabase(IServiceCollection services)
        {
            services.AddDatabaseModule(Configuration);
        }
    }
}
