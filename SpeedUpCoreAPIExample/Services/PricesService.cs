using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.Models;
using SpeedUpCoreAPIExample.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Services
{
    public class PricesService : IPricesService
    {
        private readonly IPricesRepository _pricesRepository;
        private readonly IPricesCacheRepository _pricesCacheRepository;

        public PricesService(IPricesRepository pricesRepository, IPricesCacheRepository pricesCacheRepository)
        {
            _pricesRepository = pricesRepository;
            _pricesCacheRepository = pricesCacheRepository;
        }

        public async Task<IEnumerable<PriceViewModel>> GetPricesAsync(int productId)
        {
            IEnumerable<Price> pricess = await _pricesCacheRepository.GetOrSetValueAsync(productId.ToString(), 
                async () =>  await _pricesRepository.GetPricesAsync(productId));

            return pricess.Select(p => new PriceViewModel(p))
                                .OrderBy(p => p.Price)
                                .ThenBy(p => p.Supplier);
        }

        public async Task<bool> IsPriceCachedAsync(int productId)
        {
            return await _pricesCacheRepository.IsValueCachedAsync(productId.ToString());
        }

        public async Task RemovePriceAsync(int productId)
        {
            await _pricesCacheRepository.RemoveValueAsync(productId.ToString());
        }

        public async Task PreparePricesAsync(int productId)
        {
            try
            {
                await _pricesCacheRepository.GetOrSetValueAsync(productId.ToString(), async () => await _pricesRepository.GetPricesAsync(productId));
            }
            catch
            {
            }
        }
    }
}