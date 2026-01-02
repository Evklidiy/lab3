using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class SupplyBatch
    {
        [Key]
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}