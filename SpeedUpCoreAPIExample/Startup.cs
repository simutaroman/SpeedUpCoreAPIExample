using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpeedUpCoreAPIExample.Contexts;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.Repositories;
using SpeedUpCoreAPIExample.Services;

namespace SpeedUpCoreAPIExample
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
            services.AddDbContext<DefaultContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultDatabase")));

            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IPricesRepository, PricesRepository>();

            services.AddTransient<IProductsService, ProductsService>();
            services.AddTransient<IPricesService, PricesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
