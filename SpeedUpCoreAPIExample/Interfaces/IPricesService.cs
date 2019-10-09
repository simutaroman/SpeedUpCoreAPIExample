using Microsoft.AspNetCore.Mvc;
using SpeedUpCoreAPIExample.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Interfaces
{
    public interface IPricesService
    {
        Task<IEnumerable<PriceViewModel>> GetPricesAsync(int productId);
        Task PreparePricesAsync(int productId);
    }
}