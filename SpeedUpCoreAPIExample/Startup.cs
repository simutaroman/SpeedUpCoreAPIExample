using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpeedUpCoreAPIExample.Contexts;
using SpeedUpCoreAPIExample.Exceptions;
using SpeedUpCoreAPIExample.Filters;
using SpeedUpCoreAPIExample.Helpers;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.Repositories;
using SpeedUpCoreAPIExample.Services;
using SpeedUpCoreAPIExample.Settings;

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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddHttpClient<ISelfHttpClient, SelfHttpClient>();

            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = Configuration.GetValue<string>("Redis:Name");
                options.Configuration = Configuration.GetValue<string>("Redis:Host");
            });

            services.Configure<ProductsSettings>(Configuration.GetSection("Products"));
            services.Configure<PricesSettings>(Configuration.GetSection("Prices"));

            services.AddDbContext<DefaultContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultDatabase")));

            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IPricesRepository, PricesRepository>();
            services.AddScoped<IPricesCacheRepository, PricesCacheRepository>();
            services.AddScoped<IProductCacheRepository, ProductCacheRepository>();
            
            services.AddSingleton<ValidateIdAsyncActionFilter>();
            services.AddSingleton<ValidatePagingAsyncActionFilter>();

            services.AddTransient<IProductsService, ProductsService>();
            services.AddTransient<IPricesService, PricesService>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/Log.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionsHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
