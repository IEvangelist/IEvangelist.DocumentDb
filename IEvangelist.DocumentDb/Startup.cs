using IEvangelist.DocumentDb.Models;
using IEvangelist.DocumentDb.Repository;
using IEvangelist.DocumentDb.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;
using System.IO;

namespace IEvangelist.DocumentDb
{
    public class Startup
    {
        internal static IWebHost BuildWebHost()
            => new WebHostBuilder()
                   .UseKestrel()
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseIISIntegration()
                   .UseStartup<Startup>()
                   .Build();

        internal IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
            => Configuration =
                new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Title = "IEvangelist.DocumentDb API",
                    Description = "Demonstrating Microsoft's NoSQL -- Azure DocumentDb",
                    Version = "v1"
                });
            });

            services.Configure<RepositorySettings>(
                Configuration.GetSection(nameof(RepositorySettings)));

            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IHostingEnvironment env, 
                              ILoggerFactory loggerFactory,
                              IRepository<Person> personRepo)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger()
               .UseSwaggerUi()
               .UseMvc(routes =>
               {
                   routes.MapRoute(
                       name: "default",
                       template: "{controller=Home}/{action=Index}/{id?}");
               });

            personRepo.InitializeAsync().Wait();
        }
    }
}