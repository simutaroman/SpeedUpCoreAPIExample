using Microsoft.AspNetCore.Mvc;
using SpeedUpCoreAPIExample.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Interfaces
{
    public interface IPricesService
    {
        Task<IEnumerable<PriceViewModel>> GetPricesAsync(int productId);
        Task<bool> IsPriceCachedAsync(int productId);
        Task RemovePriceAsync(int productId);
        Task PreparePricesAsync(int productId);
    }
}