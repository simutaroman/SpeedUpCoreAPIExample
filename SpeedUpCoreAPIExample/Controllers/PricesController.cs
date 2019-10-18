using Microsoft.AspNetCore.Mvc;
using SpeedUpCoreAPIExample.Filters;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.ViewModels;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Controllers
{
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/[controller]/")]
    [ApiController]
    [ValidateId]
    public class PricesController : ControllerBase
    {
        private readonly IPricesService _pricesService;

        public PricesController(IPricesService pricesService)
        {
            _pricesService = pricesService;
        }

        // GET /api/prices/1
        [HttpGet("{id}")]
        [ValidatePaging]
        public async Task<IActionResult> GetPricesAsync(int id, int pageIndex, int pageSize)
        {
            PricesPageViewModel pricesPageViewModel = await _pricesService.GetPricesAsync(id, pageIndex, pageSize);

            return new OkObjectResult(pricesPageViewModel);
        }

        // POST api/prices/prepare/5
        [HttpPost("prepare/{id}")]
        public async Task<IActionResult> PreparePricessAsync(int id)
        {
            await _pricesService.PreparePricesAsync(id);

            return Ok();
        }
    }
}