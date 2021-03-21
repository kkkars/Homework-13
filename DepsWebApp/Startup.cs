using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using DepsWebApp.Clients;
using DepsWebApp.Options;
using DepsWebApp.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DepsWebApp.Middlewares;
using DepsWebApp.Filters;
using System.IO;
using DepsWebApp.Authentication;
using Microsoft.EntityFrameworkCore;

namespace DepsWebApp
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
            // Add AccountServiceInMemory as Singleton
            services.AddTransient<IDbService, DatabaseService>();

            // Add options
            services
                .Configure<CacheOptions>(Configuration.GetSection("Cache"))
                .Configure<NbuClientOptions>(Configuration.GetSection("Client"))
                .Configure<RatesOptions>(Configuration.GetSection("Rates"));

            // Add application services
            services.AddScoped<IRatesService, RatesService>();

            //Add CustomExceptionFilter
            services.AddMvc(options => options.Filters.Add(new CustomExceptionFilter()));

            // Add NbuClient as Transient
            services.AddHttpClient<IRatesProviderClient, NbuClient>()
                .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(10));

            // Add CacheHostedService as Singleton
            services.AddHostedService<CacheHostedService>();

            services
               .AddAuthentication(CustomAuthSchema.Name)
               .AddScheme<CustomAuthSchemaOptions, CustomAuthSchemaHandler>(
                   CustomAuthSchema.Name, CustomAuthSchema.Name, null);

            // Disable ModelStateInvalidFilter and MapClientErrors
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;
            });

            // Add batch of Swashbuckle Swagger services
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DI Demo App API", Version = "v1" });
                var filePath = Path.Combine(AppContext.BaseDirectory, "DepsWebAppComments.xml");
                if (File.Exists(filePath))
                {
                    c.IncludeXmlComments(filePath, includeControllerXmlComments: true);
                }

                c.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "Encoded string",
                                        Type = ReferenceType.SecurityScheme
                                    },
                                },
                                new string[0]
                            }
                        });

                c.AddSecurityDefinition(
                        "Encoded string",
                        new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Scheme = "Encoded string",
                            Name = "Authorization",
                            Description = "Encoded string",
                            BearerFormat = "Encoded string"
                        });
            });


            services.AddDbContext<DepsWebAppContext>(options =>
               options.UseNpgsql(Configuration.GetConnectionString("DepsWebAppContext")), ServiceLifetime.Transient);

            services.AddDatabaseDeveloperPageExceptionFilter();


            // Add batch of framework services
            services.AddMemoryCache();
            services.AddControllers();
        }

        // This method gets called by the runtime.
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DI Demo App API v1"));
            }

            app.UseRouting();

            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}