namespace AutoPartsWarehouse.Models
{
    public class SearchModel
    {
        public string QueryText { get; set; } = "";     // Текстовое поле
        public string Category { get; set; } = "Part";  // Выпадающий список
        public string SortOrder { get; set; } = "Asc";  // Радио-кнопки
    }
}