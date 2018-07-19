using EntityFramework.DbContextScope;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WynnSecurity.Api.Configurations;
using WynnSecurity.Common;
using WynnSecurity.DataAccess;
using WynnSecurity.DataAccess.Read;
using WynnSecurity.DataAccess.Write;
using WynnSecurity.DataAccess.Write.Repositories;
using WynnSecurity.Domain;
using WynnSecurity.Domain.Interfaces;
using WynnSecurity.Domain.Service;

namespace WynnSecurity.Api
{
    public class Startup
    {
        private const string ExceptionsOnStartup = "Startup";
        private const string ExceptionsOnConfigureServices = "ConfigureServices";
        private const string ErrorResponseType = "application/json";

        // Captures exceptions occur on Startup and ConfigureServices 
        private readonly Dictionary<string, List<Exception>> _exceptions;

        public Startup(IConfiguration configuration)
        {
            _exceptions = new Dictionary<string, List<Exception>>
            {
                { ExceptionsOnStartup, new List<Exception>() },
                { ExceptionsOnConfigureServices, new List<Exception>() }
            };

            Configuration = configuration;
            
            try
            {
                SwaggerConfig = new SwaggerConfig();
                Configuration.GetSection("SwaggerConfig").Bind(SwaggerConfig);
            }
            catch(Exception ex)
            {
                _exceptions[ExceptionsOnStartup].Add(ex);
            }
        }

        public IConfiguration Configuration { get; }

        public SwaggerConfig SwaggerConfig { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                InitializeDbConfiguration(services);
                AddSwaggerGen(services);

                InjectDbContextScope(services);
                InjectDomainSevices(services);
                InjectRepositories(services);

                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            }
            catch(Exception ex)
            {
                _exceptions[ExceptionsOnConfigureServices].Add(ex);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Add NLog to the application
            loggerFactory.AddNLog();
            env.ConfigureNLog($"nlog.{env.EnvironmentName}.config");

            // Check if any errors occurred on the constructor or ConfigureServices
            var logger = loggerFactory.CreateLogger<Startup>();
            if (_exceptions.Any(p => p.Value.Any()))
            {
                app.Run(context => HandleStartupErrors(context, logger, _exceptions));
                return;
            }

            try
            {
                app.UseExceptionHandler(builder =>
                {
                    // Add exception handling middleware which hanles all runtime errors. 
                    builder.Run(HandleRuntimeErrors);
                });

                if (SwaggerConfig.IsEnabled)
                {
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wynn Security V1");
                        c.RoutePrefix = "swagger";

                    });

                    app.UseSwagger();
                }

                app.UseWhen(context => {
                    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = 1;
                    return true;
                    }, appBuilder =>
                {
                    //TODO: take next steps
                });

                app.UseHttpsRedirection();
                app.UseMvc();
            }
            catch(Exception ex)
            {
                app.Run(async context =>
                {
                    await WriteErrorResponseAsync(context, ex.GetBaseException().Message).ConfigureAwait(false);
                });
            }
        }

        private static async Task HandleStartupErrors(HttpContext context, ILogger logger, Dictionary<string, List<Exception>> exceptions)
        {
            var errorMessages = new List<string>();
            foreach(var key in exceptions.Keys)
            {
                foreach(var ex in exceptions[key])
                {
                    var errorMessage = $"{key} - {ex.GetBaseException().Message}";
                    errorMessages.Add(errorMessage);
                    logger.LogError(errorMessage);
                }
            }

            await WriteErrorResponseAsync(context, string.Join(Environment.NewLine, errorMessages)).ConfigureAwait(false);
        }

        private static async Task HandleRuntimeErrors(HttpContext context)
        {
            var ehf = context.Features.Get<IExceptionHandlerFeature>();
            if (ehf != null)
            {
                await WriteErrorResponseAsync(context, ehf.Error.GetBaseException().Message).ConfigureAwait(false);
            }
        }

        private static async Task WriteErrorResponseAsync(HttpContext context, string errorMessage)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = ErrorResponseType;

            var errorResponse = new ErrorResponse((int)HttpStatusCode.InternalServerError, errorMessage);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse)).ConfigureAwait(false);
        }

        private void InitializeDbConfiguration(IServiceCollection services)
        {
            var dbConnectionStrings = new DbConnectionStrings();

            Configuration.GetSection("ConnectionStrings").Bind(dbConnectionStrings);

            services.AddDbContextPool<ReadDbContext>(options =>
            {
                options.UseSqlServer(dbConnectionStrings.ConnectionString);
            });

            services.AddDbContextPool<WynnDbContext>(options => options.UseSqlServer(dbConnectionStrings.ConnectionString));
        }

        private void AddSwaggerGen(IServiceCollection services)
        {
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info { Title = "Wynn Security Api", Version = "v1" }));
        }

        private void InjectDbContextScope(IServiceCollection services)
        {
            services.AddSingleton<IDbContextFactory, DbContextFactory>();
            services.AddSingleton<IDbContextScopeFactory, DbContextScopeFactory>();
            services.AddSingleton<IWynnContextScopeFactory, WynnContextScopeFactory>();
            services.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();

            services.AddScoped<IReadDbFacade, ReadDbFacade>();
        }

        private void InjectDomainSevices(IServiceCollection services)
        {
            services.AddTransient<ICustomerService, CustomerService>();
        }

        private void InjectRepositories(IServiceCollection services)
        {
            services.AddTransient<ICustomerRepository, CustomerRepository>();
        }
    }
}
