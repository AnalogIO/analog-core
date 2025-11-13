using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Azure.Monitor.OpenTelemetry.Exporter;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.MobilePay.Utils;
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
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;
using AccountService = CoffeeCard.Library.Services.AccountService;
using IAccountService = CoffeeCard.Library.Services.IAccountService;
using IPurchaseService = CoffeeCard.Library.Services.IPurchaseService;
using ITicketService = CoffeeCard.Library.Services.ITicketService;
using PurchaseService = CoffeeCard.Library.Services.PurchaseService;
using TicketService = CoffeeCard.Library.Services.TicketService;

namespace CoffeeCard.WebApi
{
    /// <summary>
    /// The class that initializes the application by configuring services and the app's request pipeline.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// The class that initializes the application by configuring services and the app's request pipeline.
        /// </summary>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _environment = env;
        }

        /// This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfigurationSettings(_configuration);

            // Setup database connection
            var databaseSettings = _configuration
                .GetSection(nameof(DatabaseSettings))
                .Get<DatabaseSettings>();
            services.AddDbContext<CoffeeCardContext>(opt =>
                opt.UseSqlServer(
                    databaseSettings.ConnectionString,
                    c =>
                        c.MigrationsHistoryTable(
                            "__EFMigrationsHistory",
                            databaseSettings.SchemaName
                        )
                )
            );

            // Setup cache
            services.AddMemoryCache();

            // Setup Dependency Injection
            services.AddSingleton(_environment);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IHashService, HashService>();
            services.AddTransient<Library.Services.ITokenService, Library.Services.TokenService>();
            services.AddScoped<
                Library.Services.v2.ITokenService,
                Library.Services.v2.TokenService
            >();
            services.AddSingleton<ILoginLimiter, LoginLimiter>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<
                Library.Services.v2.IAccountService,
                Library.Services.v2.AccountService
            >();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IMapperService, MapperService>();
            if (_environment.IsDevelopment())
            {
                services.AddTransient<IEmailSender, SmtpEmailSender>();
                services.AddSingleton(
                    _configuration.GetSection("SmtpSettings").Get<SmtpSettings>()
                );
            }
            else
            {
                services.AddTransient<IEmailSender, MailgunEmailSender>();
            }
            services.AddScoped<Library.Services.IEmailService, Library.Services.EmailService>();
            services.AddScoped<
                Library.Services.v2.IEmailService,
                Library.Services.v2.EmailService
            >();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IProgrammeService, ProgrammeService>();
            services.AddScoped<Library.Services.IProductService, Library.Services.ProductService>();
            services.AddScoped<
                Library.Services.v2.IProductService,
                Library.Services.v2.ProductService
            >();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ClaimsUtilities>();
            services.AddSingleton(_environment.ContentRootFileProvider);

            services.AddSingleton<IRestClient>(provider =>
            {
                var mailgunSettings = provider.GetRequiredService<MailgunSettings>();
                var options = new RestClientOptions(mailgunSettings.MailgunApiUrl)
                {
                    Authenticator = new HttpBasicAuthenticator("api", mailgunSettings.ApiKey),
                };
                return new RestClient(options);
            });

            services.AddScoped<
                Library.Services.v2.IPurchaseService,
                Library.Services.v2.PurchaseService
            >();
            services.AddScoped<
                Library.Services.v2.ITicketService,
                Library.Services.v2.TicketService
            >();
            services.AddTransient<IMobilePayAccessTokenService, MobilePayAccessTokenService>();
            services.AddMobilePayHttpClients(
                _configuration.GetSection("MobilePaySettings").Get<MobilePaySettings>()
            );
            services.AddScoped<IMobilePayPaymentsService, MobilePayPaymentsService>();
            services.AddScoped<IMobilePayWebhooksService, MobilePayWebhooksService>();
            services.AddScoped<IWebhookService, WebhookService>();
            services.AddScoped<
                Library.Services.v2.ILeaderboardService,
                Library.Services.v2.LeaderboardService
            >();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IAdminStatisticsService, AdminStatisticsService>();
            services.AddFeatureManagement();

            // Azure Application Insights/ OpenTelemetry
            var otlpSettings = _configuration.GetSection("OtlpSettings").Get<OtlpSettings>();
            var applicationInsightsConnectionString = _configuration
                .GetRequiredSection("ApplicationInsights")
                .GetValue<string>("ConnectionString");
            var environment = _configuration
                .GetSection("EnvironmentSettings")
                .Get<EnvironmentSettings>();
            var openTelemetryBuilder = services
                .AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        // Metrics provides by ASP.NET Core in .NET 8
                        .AddMeter("Microsoft.AspNetCore.Hosting")
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                    if (applicationInsightsConnectionString is null or "")
                        return;
                    metrics.AddAzureMonitorMetricExporter(options =>
                        options.ConnectionString = applicationInsightsConnectionString
                    );
                })
                .WithTracing(traces =>
                {
                    var builder = traces
                        .AddSqlClientInstrumentation()
                        .AddSqlClientInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation();

                    if (applicationInsightsConnectionString is null or "")
                        return;
                    builder.AddAzureMonitorTraceExporter(options =>
                        options.ConnectionString = applicationInsightsConnectionString
                    );
                });

            if (otlpSettings is not null)
            {
                var otlpExportProtocol = otlpSettings.Protocol switch
                {
                    OtelProtocol.Grpc => OtlpExportProtocol.Grpc,
                    OtelProtocol.Http => OtlpExportProtocol.HttpProtobuf,
                    _ => throw new ArgumentOutOfRangeException("Unspecified protocol for export"),
                };

                openTelemetryBuilder.UseOtlpExporter(
                    otlpExportProtocol,
                    new Uri(otlpSettings.Endpoint)
                );
                openTelemetryBuilder.ConfigureResource(resource =>
                {
                    resource.AddAttributes(
                        [
                            new KeyValuePair<string, object>(
                                "Env",
                                environment.EnvironmentType.ToString() ?? "Env not set"
                            ),
                        ]
                    );
                    resource.AddAzureAppServiceDetector();
                    resource.AddService(
                        $"analog-core-{environment.EnvironmentType}",
                        "analog-core"
                    );
                });
                if (otlpSettings.Protocol is OtelProtocol.Http)
                {
                    services
                        .AddHttpClient(
                            "OtlpTraceExporter",
                            client =>
                                client.DefaultRequestHeaders.Add(
                                    "Authorization",
                                    $"Basic {otlpSettings.Token}"
                                )
                        )
                        .RemoveAllLoggers();
                    services
                        .AddHttpClient(
                            "OtlpMetricExporter",
                            client =>
                                client.DefaultRequestHeaders.Add(
                                    "Authorization",
                                    $"Basic {otlpSettings.Token}"
                                )
                        )
                        .RemoveAllLoggers();
                }
            }

            // Setup filter to catch outgoing exceptions
            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new ApiExceptionFilter());
                    options.Filters.Add(new ReadableBodyFilter());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        JsonIgnoreCondition.Never;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });

            services.AddCors(options =>
                options.AddDefaultPolicy(builder =>
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                )
            );

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
            services.AddServerSideBlazor();

            // Setup Authentication
            var identitySettings = _configuration
                .GetSection("IdentitySettings")
                .Get<IdentitySettings>();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "jwt";
                    options.DefaultChallengeScheme = "jwt";
                })
                .AddJwtBearer(
                    "jwt",
                    options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(identitySettings.TokenKey)
                            ),
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero, //the default for this setting is 5 minutes
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                if (
                                    context.Exception.GetType()
                                    == typeof(SecurityTokenExpiredException)
                                )
                                    context.Response.Headers.Append("Token-Expired", "true");

                                return Task.CompletedTask;
                            },
                        };
                    }
                )
                .AddApiKeyInHeaderOrQueryParams(
                    "apikey",
                    options =>
                    {
                        options.Realm = "Analog Core";
                        options.KeyName = "x-api-key";
                        options.Events = new ApiKeyEvents
                        {
                            OnValidateKey = context =>
                            {
                                var identitySettings = _configuration
                                    .GetSection(nameof(IdentitySettings))
                                    .Get<IdentitySettings>();
                                var apiKey = identitySettings.ApiKey;
                                if (apiKey == context.ApiKey)
                                {
                                    context.ValidationSucceeded();
                                    return Task.CompletedTask;
                                }
                                else
                                {
                                    context.ValidationFailed();
                                    return Task.CompletedTask;
                                }
                            },
                        };
                    }
                );
        }

        /// <summary>
        /// Generate Open Api Document for each API Version
        /// </summary>
        private static void GenerateOpenApiDocument(IServiceCollection services)
        {
            var apiVersions = services
                .BuildServiceProvider()
                .GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var apiVersion in apiVersions.ApiVersionDescriptions)
            {
                // Add an OpenApi document per API version
                services.AddOpenApiDocument(config =>
                {
                    config.DefaultResponseReferenceTypeNullHandling =
                        ReferenceTypeNullHandling.NotNull;
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
                            Url = "https://github.com/analogio",
                        };
                        document.Info.License = new OpenApiLicense
                        {
                            Name = "Use under MIT",
                            Url = "https://github.com/AnalogIO/analog-core/blob/master/LICENSE",
                        };
                    };

                    config.DocumentProcessors.Add(
                        new SecurityDefinitionAppender(
                            "jwt",
                            new OpenApiSecurityScheme
                            {
                                Description = "JWT Bearer token",
                                Name = "Authorization",
                                Scheme = "bearer",
                                BearerFormat = "JWT",
                                In = OpenApiSecurityApiKeyLocation.Header,
                                Type = OpenApiSecuritySchemeType.Http,
                            }
                        )
                    );
                    config.OperationProcessors.Add(
                        new AspNetCoreOperationSecurityScopeProcessor("jwt")
                    );

                    config.DocumentProcessors.Add(
                        new SecurityDefinitionAppender(
                            "apikey",
                            new OpenApiSecurityScheme
                            {
                                Description = "Api Key used for health endpoints",
                                Name = "x-api-key",
                                In = OpenApiSecurityApiKeyLocation.Header,
                                Type = OpenApiSecuritySchemeType.ApiKey,
                            }
                        )
                    );
                    config.OperationProcessors.Add(
                        new AspNetCoreOperationSecurityScopeProcessor("apikey")
                    );

                    // Assume not null as default unless parameter is marked as nullable
                    // config.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
                });
            }
        }

        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider
        )
        {
            // Important note!
            // The order of the below app configuration is sensitive and should be changed with care
            // UsePathBase must be first as several subsequent configuration depends on it
            app.UsePathBase("/coffeecard");

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseOpenApi();
            app.UseSwaggerUi();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            var featureManager = app.ApplicationServices.GetRequiredService<IFeatureManager>();

            var isRequestLoggerEnabled = featureManager
                .IsEnabledAsync(FeatureFlags.RequestLoggerEnabled)
                .Result;
            if (isRequestLoggerEnabled)
            {
                app.UseMiddleware<RequestLoggerMiddleware>();
            }

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSerilogRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapFallbackToPage("/result");
            });

            // Enable Request Buffering so that a raw request body can be read after aspnet model binding
            app.Use(next =>
                context =>
                {
                    context.Request.EnableBuffering();
                    return next(context);
                }
            );
        }
    }
}
