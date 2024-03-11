using AspNetCore.Authentication.ApiKey;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.MobilePay.Utils;
using CoffeeCard.WebApi.Helpers;
using Microsoft.ApplicationInsights;
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
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Generation.Processors.Security;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AccountService = CoffeeCard.Library.Services.AccountService;
using IAccountService = CoffeeCard.Library.Services.IAccountService;
using IPurchaseService = CoffeeCard.Library.Services.IPurchaseService;
using ITicketService = CoffeeCard.Library.Services.ITicketService;
using PurchaseService = CoffeeCard.Library.Services.PurchaseService;
using TicketService = CoffeeCard.Library.Services.TicketService;

namespace CoffeeCard.WebApi
{
#pragma warning disable CS1591
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfigurationSettings(_configuration);

            // Setup database connection
            var databaseSettings = _configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
            _ = services.AddDbContext<CoffeeCardContext>(opt =>
                opt.UseSqlServer(databaseSettings.ConnectionString,
                    c => c.MigrationsHistoryTable("__EFMigrationsHistory", databaseSettings.SchemaName)));

            // Setup cache
            _ = services.AddMemoryCache();

            // Setup Dependency Injection
            _ = services.AddSingleton(_environment);
            _ = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _ = services.AddScoped<IHashService, HashService>();
            _ = services.AddTransient<ITokenService, TokenService>();
            _ = services.AddSingleton<ILoginLimiter, LoginLimiter>();
            _ = services.AddScoped<IAccountService, AccountService>();
            _ = services.AddScoped<Library.Services.v2.IAccountService, Library.Services.v2.AccountService>();
            _ = services.AddScoped<IPurchaseService, PurchaseService>();
            _ = services.AddScoped<IMapperService, MapperService>();
            _ = services.AddScoped<IEmailService, EmailService>();
            _ = services.AddScoped<IVoucherService, VoucherService>();
            _ = services.AddScoped<IProgrammeService, ProgrammeService>();
            _ = services.AddScoped<Library.Services.IProductService, Library.Services.ProductService>();
            _ = services.AddScoped<Library.Services.v2.IProductService, Library.Services.v2.ProductService>();
            _ = services.AddScoped<IMenuItemService, MenuItemService>();
            _ = services.AddScoped<ITicketService, TicketService>();
            _ = services.AddScoped<ClaimsUtilities>();
            _ = services.AddSingleton(_environment.ContentRootFileProvider);

            _ = services.AddScoped<Library.Services.v2.IPurchaseService, Library.Services.v2.PurchaseService>();
            _ = services.AddScoped<Library.Services.v2.ITicketService, Library.Services.v2.TicketService>();
            services.AddMobilePayHttpClients(_configuration.GetSection("MobilePaySettingsV2").Get<MobilePaySettingsV2>());
            _ = services.AddScoped<IMobilePayPaymentsService, MobilePayPaymentsService>();
            _ = services.AddScoped<IMobilePayWebhooksService, MobilePayWebhooksService>();
            _ = services.AddScoped<IWebhookService, WebhookService>();
            _ = services.AddScoped<Library.Services.v2.ILeaderboardService, Library.Services.v2.LeaderboardService>();
            _ = services.AddScoped<IStatisticService, StatisticService>();
            _ = services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            _ = services.AddFeatureManagement();

            // Azure Application Insights
            _ = services.AddApplicationInsightsTelemetry();
            _ = services.AddSingleton<TelemetryClient>();

            // Setup filter to catch outgoing exceptions
            _ = services.AddControllers(options =>
                {
                    options.Filters.Add(new ApiExceptionFilter());
                    options.Filters.Add(new ReadableBodyFilter());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            _ = services.AddCors(options => options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            _ = services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            _ = services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
            _ = services.Configure<ApiBehaviorOptions>(config =>
            {
                config.SuppressMapClientErrors = true;
            });

            GenerateOpenApiDocument(services);

            // Setup razor pages
            _ = services.AddRazorPages();
            _ = services.AddServerSideBlazor();

            // Setup Authentication
            var identitySettings = _configuration.GetSection("IdentitySettings").Get<IdentitySettings>();
            _ = services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "jwt";
                    options.DefaultChallengeScheme = "jwt";
                }).AddJwtBearer("jwt", options =>
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
                })
                .AddApiKeyInHeaderOrQueryParams("apikey", options =>
                {
                    options.Realm = "Analog Core";
                    options.KeyName = "x-api-key";
                    options.Events = new ApiKeyEvents
                    {
                        OnValidateKey = async context =>
                        {
                            var identitySettings = _configuration.GetSection(nameof(IdentitySettings)).Get<IdentitySettings>();
                            var apiKey = identitySettings.ApiKey;
                            if (apiKey == context.ApiKey)

                            {
                                context.ValidationSucceeded();
                            }
                            else
                            {
                                context.ValidationFailed();
                            }
                        }
                    };
                });
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
                _ = services.AddOpenApiDocument(config =>
                {
                    config.DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
                    config.Title = apiVersion.GroupName;
                    config.Version = apiVersion.ApiVersion.ToString();
                    config.DocumentName = apiVersion.GroupName;
                    config.ApiGroupNames = new[] { apiVersion.GroupName };
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

                    config.DocumentProcessors.Add(new SecurityDefinitionAppender("jwt", new OpenApiSecurityScheme
                    {
                        Description = "JWT Bearer token",
                        Name = "Authorization",
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Type = OpenApiSecuritySchemeType.Http
                    }));
                    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("jwt"));

                    config.DocumentProcessors.Add(new SecurityDefinitionAppender("apikey", new OpenApiSecurityScheme
                    {
                        Description = "Api Key used for health endpoints",
                        Name = "x-api-key",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Type = OpenApiSecuritySchemeType.ApiKey
                    }));
                    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("apikey"));

                    // Assume not null as default unless parameter is marked as nullable
                    // config.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            // Important note!
            // The order of the below app configuration is sensitive and should be changed with care
            // UsePathBase must be first as several subsequent configuration depends on it
            _ = app.UsePathBase("/coffeecard");

            if (env.IsDevelopment())
                _ = app.UseDeveloperExceptionPage();
            else
                _ = app.UseHsts();

            _ = app.UseOpenApi();
            _ = app.UseSwaggerUi();

            _ = app.UseHttpsRedirection();

            _ = app.UseStaticFiles();

            _ = app.UseRouting();

            _ = app.UseCors();

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllers();
                _ = endpoints.MapRazorPages();
                _ = endpoints.MapFallbackToPage("/result");
            });

            // Enable Request Buffering so that a raw request body can be read after aspnet model binding
            _ = app.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });
        }
    }
#pragma warning restore CS1591
}
