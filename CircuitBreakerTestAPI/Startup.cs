using CircuitBreakerTestAPI.Interfaces;
using CircuitBreakerTestAPI.Respository;
using CircuitBreakerTestAPI.Respository.ConfigurationOptions;
using CircuitBreakerTestAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace CircuitBreakerTestAPI
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CircuitBreakerTestAPI", Version = "v1" });
            });

            services.Configure<DatabaseAppSettingsOptions>(Configuration.GetSection("ConnectionStrings"));
            services.AddTransient<ITesteService, TesteService>();
            services.AddTransient<ITesteRepository, TesteRepository>();
            services.AddSingleton(new PersistencePolicy(Log.Logger, 1, 1));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CircuitBreakerTestAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
