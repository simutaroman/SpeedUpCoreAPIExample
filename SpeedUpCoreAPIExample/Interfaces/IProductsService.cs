using Microsoft.AspNetCore.Mvc;
using SpeedUpCoreAPIExample.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Interfaces
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
        Task<ProductViewModel> GetProductAsync(int productId);
        Task<IEnumerable<ProductViewModel>> FindProductsAsync(string sku);
        Task<ProductViewModel> DeleteProductAsync(int productId);
    }
}