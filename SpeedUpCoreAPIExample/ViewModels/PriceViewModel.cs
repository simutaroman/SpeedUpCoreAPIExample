using SpeedUpCoreAPIExample.Models;

namespace SpeedUpCoreAPIExample.ViewModels
{
    public class PriceViewModel
    {
        public PriceViewModel(Price price)
        {
            Price = price.Value;
            Supplier = price.Supplier;
        }
        public decimal Price { get; set; }
        public string Supplier { get; set; }
    }
}