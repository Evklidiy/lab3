using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        public int SparePartId { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
    }
}