using AutoPartsWarehouse.Models;

namespace AutoPartsWarehouse.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AutoPartsContext context)
        {
            context.Database.EnsureCreated();

            if (context.SpareParts.Any()) return; // База уже заполнена

            // Генерация 20 записей для каждой таблицы
            for (int i = 1; i <= 20; i++)
            {
                context.SpareParts.Add(new SparePart
                {
                    Article = $"ART-{i}",
                    Name = $"Деталь {i}",
                    Manufacturer = i % 2 == 0 ? "Bosch" : "Denso",
                    CompatibleModels = "Audi, BMW",
                    PurchasePrice = 100 * i,
                    SalePrice = 150 * i
                });

                context.Suppliers.Add(new Supplier
                {
                    Name = $"ООО Поставщик {i}",
                    Contacts = $"phone-{i}",
                    Rating = (i % 5) + 1
                });

                context.SupplyBatches.Add(new SupplyBatch
                {
                    SupplierId = i,
                    Date = DateTime.Now.AddDays(-i),
                    Status = "Принято"
                });

                context.SupplyItems.Add(new SupplyItem
                {
                    SupplyBatchId = i,
                    SparePartId = i,
                    Quantity = 10,
                    PurchasePrice = 100 * i
                });

                context.Sales.Add(new Sale
                {
                    ClientName = $"Клиент {i}",
                    CarModel = $"Машина {i}",
                    Date = DateTime.Now
                });

                context.SaleItems.Add(new SaleItem
                {
                    SaleId = i,
                    SparePartId = i,
                    Quantity = 1,
                    SalePrice = 150 * i
                });

                context.Stocks.Add(new Stock
                {
                    SparePartId = i,
                    Quantity = 50,
                    Location = $"A-{i}"
                });
            }
            context.SaveChanges();
        }
    }
}