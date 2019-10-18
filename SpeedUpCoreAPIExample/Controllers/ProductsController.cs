using Microsoft.AspNetCore.Mvc;
using SpeedUpCoreAPIExample.Filters;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.ViewModels;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        // GET /api/products
        [HttpGet]
        [ValidatePaging]
        public async Task<IActionResult> GetAllProductsAsync(int pageIndex, int pageSize)
        {
            ProductsPageViewModel productsPageViewModel = await _productsService.GetAllProductsAsync(pageIndex, pageSize);

            return new OkObjectResult(productsPageViewModel);
        }

        // GET /api/products/5
        [HttpGet("{id}")]
        [ValidateId]
        public async Task<IActionResult> GetProductAsync(int id)
        {
            return new OkObjectResult(await _productsService.GetProductAsync(id));
        }

        // GET /api/products/find
        [HttpGet("find/{sku}")]
        [ValidatePaging]
        public async Task<IActionResult> FindProductsAsync(string sku, int pageIndex, int pageSize)
        {
            ProductsPageViewModel productsPageViewModel = await _productsService.FindProductsAsync(sku, pageIndex, pageSize);

            return new OkObjectResult(productsPageViewModel);
        }

        // DELETE /api/products/5
        [HttpDelete("{id}")]
        [ValidateId]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            return new OkObjectResult(await _productsService.DeleteProductAsync(id));
        }
    }
}