﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Models;

namespace CoffeeCard.Library.Services
{
    public interface IProductService : IDisposable
    {
        Task<IEnumerable<Product>> GetPublicProducts();
        Task<IEnumerable<Product>> GetProductsForUserAsync(User user);
    }
}