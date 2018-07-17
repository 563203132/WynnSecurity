using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WynnSecurity.Common;
using WynnSecurity.DataAccess;
using WynnSecurity.DataAccess.Repositories;
using WynnSecurity.Domain;
using WynnSecurity.Domain.Service;

namespace WynnSecurity.Api
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
            InitializeDbConfiguration(services);

            InjectDomainSevices(services);
            InjectRepositories(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void InitializeDbConfiguration(IServiceCollection services)
        {
            var dbConnectionStrings = new DbConnectionStrings();

            Configuration.GetSection("ConnectionStrings").Bind(dbConnectionStrings);

            services.AddDbContextPool<WynnDbContext>((options => options.UseSqlServer(dbConnectionStrings.ConnectionString)));
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
