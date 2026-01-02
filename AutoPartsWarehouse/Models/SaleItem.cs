using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class SaleItem
    {
        [Key]
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int SparePartId { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
    }
}