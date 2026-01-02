using System.ComponentModel.DataAnnotations;

namespace AutoPartsWarehouse.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contacts { get; set; }
        public int Rating { get; set; }
    }
}