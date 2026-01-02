using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class SupplyItem
    {
        [Key]
        public int Id { get; set; }
        public int SupplyBatchId { get; set; }
        public int SparePartId { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}