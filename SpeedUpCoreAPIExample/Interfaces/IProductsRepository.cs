using SpeedUpCoreAPIExample.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Interfaces
{
    public interface IProductsRepository
    {
        IQueryable<Product> GetAllProductsAsync();
        IQueryable<Product> FindProductsAsync(string sku);
        Task<Product> GetProductAsync(int productId);
        Task<Product> DeleteProductAsync(int productId);
    }
}