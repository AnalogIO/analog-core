using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class ProductServiceTest
    {
        [Fact(DisplayName = "GetProductsForUserAsync does not return non-visible products")]
        public async Task GetProductsForUserAsync_DoesNot_Return_NonVisible_Products()
        {
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetProductsForUserAsync_DoesNot_Return_NonVisible_Products));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            Product p1 = new Product
            {
                Id = 1,
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                ExperienceWorth = 10,
                Visible = true
            };
            _ = await context.AddAsync(p1);

            Product p2 = new Product
            {
                Id = 2,
                Name = "Espresso",
                Description = "Espresso Clip card",
                NumberOfTickets = 10,
                Price = 20,
                ExperienceWorth = 20,
                Visible = false
            };
            _ = await context.AddAsync(p2);
            _ = await context.SaveChangesAsync();

            _ = await context.AddAsync(new ProductUserGroup
            {
                Product = p1,
                UserGroup = UserGroup.Barista
            });
            _ = await context.AddAsync(new ProductUserGroup
            {
                Product = p2,
                UserGroup = UserGroup.Barista
            });
            _ = await context.SaveChangesAsync();

            using ProductService productService = new ProductService(context);
            List<Product> expected =
            [
                new Product
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    Visible = true
                }
            ];

            User user = new User
            {
                UserGroup = UserGroup.Barista
            };

            IEnumerable<Product> result = await productService.GetProductsForUserAsync(user);

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "GetProductsForUserAsync return products for usergroup")]
        public async Task GetProductsForUserAsync_Return_Products_For_UserGroup()
        {
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetProductsForUserAsync_Return_Products_For_UserGroup));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            Product p1 = new Product
            {
                Id = 1,
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                ExperienceWorth = 10,
                Visible = true
            };
            _ = await context.AddAsync(p1);

            Product p2 = new Product
            {
                Id = 2,
                Name = "Espresso",
                Description = "Espresso Clip card",
                NumberOfTickets = 10,
                Price = 20,
                ExperienceWorth = 20,
                Visible = true
            };
            _ = await context.AddAsync(p2);

            Product p3 = new Product
            {
                Id = 3,
                Name = "Barista Coffee",
                Description = "Barista Coffee Clip card",
                NumberOfTickets = 10,
                Price = 30,
                ExperienceWorth = 10,
                Visible = true
            };
            _ = await context.AddAsync(p3);
            _ = await context.SaveChangesAsync();

            _ = await context.AddAsync(new ProductUserGroup
            {
                Product = p1,
                UserGroup = UserGroup.Barista
            });
            _ = await context.AddAsync(new ProductUserGroup
            {
                Product = p2,
                UserGroup = UserGroup.Barista
            });
            _ = await context.AddAsync(new ProductUserGroup
            {
                Product = p3,
                UserGroup = UserGroup.Barista
            });
            _ = await context.SaveChangesAsync();

            using (ProductService productService = new ProductService(context))
            {
                List<Product> expected =
                [
                    new Product
                    {
                        Id = 1,
                        Name = "Coffee",
                        Description = "Coffee Clip card",
                        NumberOfTickets = 10,
                        Price = 10,
                        ExperienceWorth = 10,
                        Visible = true
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Espresso",
                        Description = "Espresso Clip card",
                        NumberOfTickets = 10,
                        Price = 20,
                        ExperienceWorth = 20,
                        Visible = true
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Barista Coffee",
                        Description = "Barista Coffee Clip card",
                        NumberOfTickets = 10,
                        Price = 30,
                        ExperienceWorth = 10,
                        Visible = true
                    }
                ];

                User user = new User
                {
                    UserGroup = UserGroup.Barista
                };

                IEnumerable<Product> result = await productService.GetProductsForUserAsync(user);

                Assert.Equal(expected, result);
            }
        }

        [Fact(DisplayName = "GetPublicProducts does not return non-visible products")]
        public async Task GetPublicProducts_DoesNot_Return_NonVisible_Products()
        {
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetPublicProducts_DoesNot_Return_NonVisible_Products));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            using (CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings))
            {
                Product p1 = new Product
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    Visible = true
                };
                _ = await context.AddAsync(p1);

                Product p2 = new Product
                {
                    Id = 2,
                    Name = "Espresso",
                    Description = "Espresso Clip card",
                    NumberOfTickets = 10,
                    Price = 20,
                    ExperienceWorth = 20,
                    Visible = false
                };
                _ = await context.AddAsync(p2);
                _ = await context.SaveChangesAsync();

                _ = await context.AddAsync(new ProductUserGroup
                {
                    Product = p1,
                    UserGroup = UserGroup.Customer
                });
                _ = await context.AddAsync(new ProductUserGroup
                {
                    Product = p2,
                    UserGroup = UserGroup.Customer
                });
                _ = await context.SaveChangesAsync();

                using ProductService productService = new ProductService(context);
                List<Product> expected =
                [
                    new Product
                    {
                        Id = 1,
                        Name = "Coffee",
                        Description = "Coffee Clip card",
                        NumberOfTickets = 10,
                        Price = 10,
                        ExperienceWorth = 10,
                        Visible = true
                    }
                ];

                IEnumerable<Product> result = await productService.GetPublicProducts();

                Assert.Equal(expected, result);
            }
        }

        [Fact(DisplayName = "GetPublicProducts return all non-barista products")]
        public async Task GetPublicProducts_Return_All_NonBarista_Products()
        {
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetPublicProducts_Return_All_NonBarista_Products));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            using (CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings))
            {
                Product p1 = new Product
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    Visible = true
                };
                _ = await context.AddAsync(p1);

                Product p2 = new Product
                {
                    Id = 2,
                    Name = "Espresso",
                    Description = "Espresso Clip card",
                    NumberOfTickets = 10,
                    Price = 20,
                    ExperienceWorth = 20,
                    Visible = true
                };
                _ = await context.AddAsync(p2);

                Product p3 = new Product
                {
                    Id = 3,
                    Name = "Barista Coffee",
                    Description = "Barista Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 30,
                    ExperienceWorth = 10,
                    Visible = true
                };
                _ = await context.AddAsync(p3);
                _ = await context.SaveChangesAsync();

                _ = await context.AddAsync(new ProductUserGroup
                {
                    Product = p1,
                    UserGroup = UserGroup.Customer
                });
                _ = await context.AddAsync(new ProductUserGroup
                {
                    Product = p2,
                    UserGroup = UserGroup.Customer
                });
                _ = await context.AddAsync(new ProductUserGroup
                {
                    Product = p3,
                    UserGroup = UserGroup.Barista
                });
                _ = await context.SaveChangesAsync();

                using ProductService productService = new ProductService(context);
                List<Product> expected =
                [
                    p1,
                    p2
                ];

                IEnumerable<Product> result = await productService.GetPublicProducts();

                Assert.Equal(expected, result);
            }
        }
    }
}