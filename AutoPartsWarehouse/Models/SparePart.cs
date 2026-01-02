using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class SparePart
    {
        [Key]
        public int Id { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string CompatibleModels { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
    }
}