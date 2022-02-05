using System;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Utils;
using CoffeeCard.MobilePay.Client;
using CoffeeCard.MobilePay.Service;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;

namespace CoffeeCard.WebApi
{
#pragma warning disable CS1591
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup database connection
            var databaseSettings = Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
            services.AddDbContext<CoffeeCardContext>(opt =>
                opt.UseSqlServer(databaseSettings.ConnectionString,
                    c => c.MigrationsHistoryTable("__EFMigrationsHistory", databaseSettings.SchemaName)));

            // Setup Dependency Injection
            services.AddSingleton(Environment);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IHashService, HashService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddSingleton<ILoginLimiter, LoginLimiter>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IMapperService, MapperService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IProgrammeService, ProgrammeService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IMobilePayService, MobilePayService>();
            services.AddScoped<ILeaderboardService, LeaderboardService>();
            services.AddScoped<IAppConfigService, AppConfigService>();
            services.AddScoped<ClaimsUtilities>();
            services.AddHttpClient<IMobilePayApiHttpClient, MobilePayApiHttpClient>();
            services.AddSingleton(Environment.ContentRootFileProvider);

            // Setup filter to catch outgoing exceptions
            services.AddControllers(options => { options.Filters.Add(new ApiExceptionFilter()); })
                // Setup Json Serializing
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
            services.Configure<ApiBehaviorOptions>(config =>
            {
                config.SuppressMapClientErrors = true;
            });

                GenerateOpenApiDocument(services);

            // Setup razor pages
            services.AddRazorPages();

            // Setup Authentication
            var identitySettings = Configuration.GetSection("IdentitySettings").Get<IdentitySettings>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "bearer";
                options.DefaultChallengeScheme = "bearer";
            }).AddJwtBearer("bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(identitySettings.TokenKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero //the default for this setting is 5 minutes
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            context.Response.Headers.Add("Token-Expired", "true");

                        return Task.CompletedTask;
                    }
                };
            });

            services.UseConfigurationValidation();

            // Parse and setup settings from configuration
            services.ConfigureValidatableSetting<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));
            services.ConfigureValidatableSetting<EnvironmentSettings>(Configuration.GetSection("EnvironmentSettings"));
            services.ConfigureValidatableSetting<LoginLimiterSettings>(Configuration.GetSection("LoginLimiterSettings"));
            services.ConfigureValidatableSetting<IdentitySettings>(Configuration.GetSection("IdentitySettings"));
            services.ConfigureValidatableSetting<MailgunSettings>(Configuration.GetSection("MailgunSettings"));
            services.ConfigureValidatableSetting<MobilePaySettings>(Configuration.GetSection("MobilePaySettings"));
        }

        
        /// <summary>
        /// Generate Open Api Document for each API Version
        /// </summary>
        private static void GenerateOpenApiDocument(IServiceCollection services)
        {
            var apiVersions = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var apiVersion in apiVersions.ApiVersionDescriptions)
            {
                // Add an OpenApi document per API version
                services.AddOpenApiDocument(config =>
                {
                    config.Title = apiVersion.GroupName;
                    config.Version = apiVersion.ApiVersion.ToString();
                    config.DocumentName = apiVersion.GroupName;
                    config.ApiGroupNames = new[] {apiVersion.GroupName};
                    config.Description = "ASP.NET Core WebAPI for Cafe Analog";
                    config.PostProcess = document =>
                    {
                        document.Info.Title = "Cafe Analog CoffeeCard API";
                        document.Info.Version = $"v{apiVersion.ApiVersion}";
                        document.Info.Contact = new OpenApiContact
                        {
                            Name = "AnalogIO",
                            Email = "support@analogio.dk",
                            Url = "https://github.com/analogio"
                        };
                        document.Info.License = new OpenApiLicense
                        {
                            Name = "Use under MIT",
                            Url = "https://github.com/AnalogIO/analog-core/blob/master/LICENSE"
                        };
                    };

                    // Configure OpenApi Security scheme
                    // Allows using the Swagger UI with a Bearer token
                    config.AddSecurity("Bearer", new OpenApiSecurityScheme()
                    {
                        Description = "Insert a JWT Bearer token: Bearer {token}",
                        Name = "Authorization",
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Type = OpenApiSecuritySchemeType.Http
                    });
                    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));

                    // Assume not null as default unless parameter is marked as nullable
                    config.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseOpenApi();

            app.UseSwaggerUi3();
            app.UseReDoc(config =>
            {
                config.Path = "/redoc";
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapFallbackToPage("/result");
            });

            Log.Information("Apply Database Migrations if any");
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetService<CoffeeCardContext>();
            if (context.Database.IsSqlServer())
            {
                context.Database.Migrate();
            }
        }
    }
#pragma warning restore CS1591
}