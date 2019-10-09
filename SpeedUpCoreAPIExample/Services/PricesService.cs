using Microsoft.AspNetCore.Mvc;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.Models;
using SpeedUpCoreAPIExample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Services
{
    public class PricesService : IPricesService
    {
        private readonly IPricesRepository _pricesRepository;

        public PricesService(IPricesRepository pricesRepository)
        {
            _pricesRepository = pricesRepository;
        }

        public async Task<IEnumerable<PriceViewModel>> GetPricesAsync(int productId)
        {
            IEnumerable<Price> pricess = await _pricesRepository.GetPricesAsync(productId);

            return pricess.Select(p => new PriceViewModel(p))
            .OrderBy(p => p.Price)
            .ThenBy(p => p.Supplier);
        }

        public async Task PreparePricesAsync(int productId)
        {
            await _pricesRepository.PreparePricesAsync(productId);
        }
    }
}