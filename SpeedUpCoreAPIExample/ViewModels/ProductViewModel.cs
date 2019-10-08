using SpeedUpCoreAPIExample.Models;

namespace SpeedUpCoreAPIExample.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel(Product product)
        {
            Id = product.ProductId;
            Sku = product.Sku;
            Name = product.Name;
        }
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
    }
}