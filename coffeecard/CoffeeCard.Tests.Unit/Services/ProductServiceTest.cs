using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class ProductServiceTest
    {
        [Fact(DisplayName = "GetPublicProducts return all non-barista products")]
        public async Task GetPublicProducts_Return_All_NonBarista_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetPublicProducts_Return_All_NonBarista_Products));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };

            using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var p1 = new Product()
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    Visible = true
                };
                await context.AddAsync(p1);

                var p2 = new Product()
                {
                    Id = 2,
                    Name = "Espresso",
                    Description = "Espresso Clip card",
                    NumberOfTickets = 10,
                    Price = 20,
                    ExperienceWorth = 20,
                    Visible = true
                };
                await context.AddAsync(p2);

                var p3 = new Product()
                {
                    Id = 3,
                    Name = "Barista Coffee",
                    Description = "Barista Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 30,
                    ExperienceWorth = 10,
                    Visible = true
                };
                await context.AddAsync(p3);
                await context.SaveChangesAsync();

                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p1,
                    UserGroup = UserGroup.Customer
                });
                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p2,
                    UserGroup = UserGroup.Customer
                });
                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p3,
                    UserGroup = UserGroup.Barista
                });
                await context.SaveChangesAsync();

                using (var productService = new ProductService(context))
                {
                    var expected = new List<Product>
                    {
                        p1,
                        p2
                    };

                    var result = await productService.GetPublicProducts();

                    Assert.Equal(expected, result);
                }
            }
        }

        [Fact(DisplayName = "GetPublicProducts does not return non-visible products")]
        public async Task GetPublicProducts_DoesNot_Return_NonVisible_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetPublicProducts_DoesNot_Return_NonVisible_Products));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };

            using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var p1 = new Product()
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    Visible = true
                };
                await context.AddAsync(p1);

                var p2 = new Product()
                {
                    Id = 2,
                    Name = "Espresso",
                    Description = "Espresso Clip card",
                    NumberOfTickets = 10,
                    Price = 20,
                    ExperienceWorth = 20,
                    Visible = false
                };
                await context.AddAsync(p2);
                await context.SaveChangesAsync();

                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p1,
                    UserGroup = UserGroup.Customer
                });
                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p2,
                    UserGroup = UserGroup.Customer
                });
                await context.SaveChangesAsync();

                using (var productService = new ProductService(context))
                {
                    var expected = new List<Product>
                    {
                        new Product()
                        {
                            Id = 1,
                            Name = "Coffee",
                            Description = "Coffee Clip card",
                            NumberOfTickets = 10,
                            Price = 10,
                            ExperienceWorth = 10,
                            Visible = true
                        },
                    };

                    var result = await productService.GetPublicProducts();

                    Assert.Equal(expected, result);
                }
            }
        }

        [Fact(DisplayName = "GetProductsForUserAsync return products for usergroup")]
        public async Task GetProductsForUserAsync_Return_Products_For_UserGroup()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetProductsForUserAsync_Return_Products_For_UserGroup));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };

            using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var p1 = new Product()
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    Visible = true
                };
                await context.AddAsync(p1);

                var p2 = new Product()
                {
                    Id = 2,
                    Name = "Espresso",
                    Description = "Espresso Clip card",
                    NumberOfTickets = 10,
                    Price = 20,
                    ExperienceWorth = 20,
                    Visible = true
                };
                await context.AddAsync(p2);

                var p3 = new Product()
                {
                    Id = 3,
                    Name = "Barista Coffee",
                    Description = "Barista Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 30,
                    ExperienceWorth = 10,
                    Visible = true
                };
                await context.AddAsync(p3);
                await context.SaveChangesAsync();

                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p1,
                    UserGroup = UserGroup.Barista
                });
                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p2,
                    UserGroup = UserGroup.Barista
                });
                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p3,
                    UserGroup = UserGroup.Barista
                });
                await context.SaveChangesAsync();

                using (var productService = new ProductService(context))
                {
                    var expected = new List<Product>
                    {
                        new Product()
                        {
                            Id = 1,
                            Name = "Coffee",
                            Description = "Coffee Clip card",
                            NumberOfTickets = 10,
                            Price = 10,
                            ExperienceWorth = 10,
                            Visible = true
                        },
                        new Product()
                        {
                            Id = 2,
                            Name = "Espresso",
                            Description = "Espresso Clip card",
                            NumberOfTickets = 10,
                            Price = 20,
                            ExperienceWorth = 20,
                            Visible = true
                        },
                        new Product()
                        {
                            Id = 3,
                            Name = "Barista Coffee",
                            Description = "Barista Coffee Clip card",
                            NumberOfTickets = 10,
                            Price = 30,
                            ExperienceWorth = 10,
                            Visible = true
                        }
                    };

                    var user = new User
                    {
                        UserGroup = UserGroup.Barista
                    };

                    var result = await productService.GetProductsForUserAsync(user);

                    Assert.Equal(expected, result);
                }
            }
        }

        [Fact(DisplayName = "GetProductsForUserAsync does not return non-visible products")]
        public async Task GetProductsForUserAsync_DoesNot_Return_NonVisible_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetProductsForUserAsync_DoesNot_Return_NonVisible_Products));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };

            using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var p1 = new Product()
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    Visible = true
                };
                await context.AddAsync(p1);

                var p2 = new Product()
                {
                    Id = 2,
                    Name = "Espresso",
                    Description = "Espresso Clip card",
                    NumberOfTickets = 10,
                    Price = 20,
                    ExperienceWorth = 20,
                    Visible = false
                };
                await context.AddAsync(p2);
                await context.SaveChangesAsync();

                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p1,
                    UserGroup = UserGroup.Barista
                });
                await context.AddAsync(new ProductUserGroup()
                {
                    Product = p2,
                    UserGroup = UserGroup.Barista
                });
                await context.SaveChangesAsync();

                using (var productService = new ProductService(context))
                {
                    var expected = new List<Product>
                    {
                        new Product()
                        {
                            Id = 1,
                            Name = "Coffee",
                            Description = "Coffee Clip card",
                            NumberOfTickets = 10,
                            Price = 10,
                            ExperienceWorth = 10,
                            Visible = true
                        },
                    };

                    var user = new User
                    {
                        UserGroup = UserGroup.Barista
                    };

                    var result = await productService.GetProductsForUserAsync(user);

                    Assert.Equal(expected, result);
                }
            }
        }
    }
}
