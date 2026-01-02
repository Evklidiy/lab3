using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string CarModel { get; set; }
        public DateTime Date { get; set; }
    }
}