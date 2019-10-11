using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SpeedUpCoreAPIExample.Contexts;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Repositories
{
    public class PricesRepository : IPricesRepository
    {
        private readonly DefaultContext _context;

        public PricesRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Price>> GetPricesAsync(int productId)
        {
            return await _context.Prices.FromSqlRaw("[dbo].GetPricesByProductId @productId = {0}", productId).AsNoTracking().ToListAsync();
        }

        public async Task PreparePricesAsync(int productId)
        {
            await _context.Prices.FromSqlRaw("[dbo].GetPricesByProductId @productId = {0}", productId).AsNoTracking().ToListAsync();
            return;
        }
    }
}