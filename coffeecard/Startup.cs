using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coffeecard.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using coffeecard.Services;
using coffeecard.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Versioning;
using NSwag.AspNetCore;
using NJsonSchema;

namespace Coffeecard
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: find out the proper way to differ between enviroment based config files
            services.AddDbContext<CoffeecardContext>(opt => 
                opt.UseSqlServer(Configuration.GetConnectionString("CoffeecardDatabase")));
            
            services.AddSingleton<IConfiguration>(provider => Configuration);
            services.AddSingleton<IHostingEnvironment>(Environment);
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

            services.AddMvc(options =>
            {
                options.Filters.Add(new ApiExceptionFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddApiVersioning();

            services.AddSwagger();


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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero //the default for this setting is 5 minutes
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
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

            app.UseSwaggerUi3WithApiExplorer(settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
                settings.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Café Analog CoffeeCard API";
                    document.Info.Description = "ASP.NET Core web API for the coffee bar Café Analog";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "AnalogIO",
                        Email = "admin@analogio.dk",
                        Url = "https://github.com/analogio"
                    };
                    document.Info.License = new NSwag.SwaggerLicense
                    {
                        Name = "Use under MIT",
                        Url = "https://github.com/AnalogIO/analog-core/blob/master/LICENSE"
                    };
                };
            });

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
