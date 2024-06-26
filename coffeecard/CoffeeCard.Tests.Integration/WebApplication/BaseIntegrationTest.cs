using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.ApiClient.Generated;
using CoffeeCard.Tests.ApiClient.v2.Generated;
using CoffeeCard.Tests.Common.Builders;
using CoffeeCard.WebApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace CoffeeCard.Tests.Integration.WebApplication
{
    [Collection("Integration tests, to be run sequentially")]
    public abstract class BaseIntegrationTest : CustomWebApplicationFactory<Startup>, IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly IServiceScope _scope;
        private readonly HttpClient _httpClient;
        protected readonly CoffeeCardClient CoffeeCardClient;
        protected readonly CoffeeCardClientV2 CoffeeCardClientV2;
        protected readonly CoffeeCardContext Context;

        protected BaseIntegrationTest(CustomWebApplicationFactory<Startup> factory)
        {
            // Set the random seed used for generation of data in the builders
            // This ensures our tests are deterministic within a specific version of the code
            var seed = new Random(42);
            Bogus.Randomizer.Seed = seed;
            _factory = factory;
            _scope = _factory.Services.CreateScope();

            _httpClient = GetHttpClient();
            CoffeeCardClient = new CoffeeCardClient(_httpClient);
            CoffeeCardClientV2 = new CoffeeCardClientV2(_httpClient);
            Context = GetCoffeeCardContext();
        }

        private HttpClient GetHttpClient()
        {
            var client = CreateClient();

            return client;
        }

        protected async Task<User> GetAuthenticatedUserAsync()
        {
            var user = UserBuilder.DefaultCustomer().Build();
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            SetDefaultAuthHeader(user);
            return user;
        }

        protected void SetDefaultAuthHeader(User user)
        {
            var claims = new[]
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.UserGroup.ToString())
                    };
            var token = GenerateToken(claims);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var scopedServices = _scope.ServiceProvider;
            var identitySettings = scopedServices.GetRequiredService<IdentitySettings>();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(identitySettings.TokenKey)); // get token from appsettings.json

            var jwt = new JwtSecurityToken("AnalogIO",
                "Everyone",
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(24),
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt); //the method is called WriteToken but returns a string
        }

        protected void RemoveRequestHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
        }

        private CoffeeCardContext GetCoffeeCardContext()
        {
            // Create a scope to obtain a reference to the database context (ApplicationDbContext).
            var scopedServices = _scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<CoffeeCardContext>();

            // Ensure the database is cleaned for each test run
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        public override ValueTask DisposeAsync()
        {
            _scope.Dispose();
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
            return base.DisposeAsync();
        }
    }
}