using System;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Client;
using CoffeeCard.MobilePay.Service;
using CoffeeCard.WebApi.Helpers;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NSwag;

namespace CoffeeCard.WebApi
{
    public class Startup
    {
	    public IConfiguration Configuration { get; }
	    public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

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

            // Setup filter to catch outgoing exceptions
            services.AddControllers(options => { options.Filters.Add(new ApiExceptionFilter()); });

            services.AddApiVersioning();

            // Setup Swagger
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Cafe Analog CoffeeCard API";
                    document.Info.Description = "ASP.NET Core web API for the coffee bar Cafe Analog";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "AnalogIO",
                        Email = "admin@analogio.dk",
                        Url = "https://github.com/analogio"
                    };
                    document.Info.License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = "https://github.com/AnalogIO/analog-core/blob/master/LICENSE"
                    };
                };
            });

            // Setup Json Serializing
            services.AddControllers()
	            .AddNewtonsoftJson(options =>
	            {
					options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
				});

            // Setup Authentication
            IdentitySettings identitySettings = Configuration.GetSection("IdentitySettings").Get<IdentitySettings>();
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
            services.ConfigureValidatableSetting<IdentitySettings>(Configuration.GetSection("IdentitySettings"));
            services.ConfigureValidatableSetting<MailgunSettings>(Configuration.GetSection("MailgunSettings"));
            services.ConfigureValidatableSetting<MobilePaySettings>(Configuration.GetSection("MobilePaySettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
	            app.UseDeveloperExceptionPage();
            }
            else
            {
	            app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
	            endpoints.MapControllers());
        }
    }
}
