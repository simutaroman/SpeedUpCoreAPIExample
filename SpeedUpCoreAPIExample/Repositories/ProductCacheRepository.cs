﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.Models;
using SpeedUpCoreAPIExample.Settings;
using System;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Repositories
{
    public class ProductCacheRepository : DistributedCacheRepository<Product>, IProductCacheRepository
    {
        private const string KeyPrefix = "Product: ";
        private readonly ProductsSettings _settings;

        public ProductCacheRepository(IDistributedCache distributedCache, IOptions<ProductsSettings> settings) : base(distributedCache, KeyPrefix)
        {
            _settings = settings.Value;
        }

        public override async Task<Product> GetOrSetValueAsync(string key, Func<Task<Product>> valueDelegate, DistributedCacheEntryOptions options = null)
        {
            return await base.GetOrSetValueAsync(key, valueDelegate, options);
        }

        protected override DistributedCacheEntryOptions GetDefaultOptions()
        {
            //use default caching options for the class if they are not defined in options parameter
            return new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CachingExpirationPeriod)
            };
        }
    }
}