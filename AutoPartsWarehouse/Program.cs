using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Infrastructure;
using AutoPartsWarehouse.Models;
using AutoPartsWarehouse.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. НАСТРОЙКА СЕРВИСОВ (SERVICES)
// ==========================================

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddMemoryCache();

// Строка подключения из appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AutoPartsContext>(options =>
    options.UseSqlServer(connectionString));

// Регистрация сервисов
builder.Services.AddScoped<ICachedService<SparePart>, CachedService<SparePart>>();
builder.Services.AddScoped<ICachedService<Supplier>, CachedService<Supplier>>();
builder.Services.AddScoped<ICachedService<SupplyBatch>, CachedService<SupplyBatch>>();
builder.Services.AddScoped<ICachedService<SupplyItem>, CachedService<SupplyItem>>();
builder.Services.AddScoped<ICachedService<Sale>, CachedService<Sale>>();
builder.Services.AddScoped<ICachedService<SaleItem>, CachedService<SaleItem>>();
builder.Services.AddScoped<ICachedService<Stock>, CachedService<Stock>>();

var app = builder.Build();

// ==========================================
// 2. КОНВЕЙЕР ОБРАБОТКИ (MIDDLEWARE)
// ==========================================

app.UseSession();

// Инициализация БД
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AutoPartsContext>();
    DbInitializer.Initialize(context);
}

// --------------------------------------------------
// МАРШРУТЫ (Сначала описываем конкретные страницы)
// --------------------------------------------------

// 1. Инфо (/info)
app.Map("/info", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "text/html;charset=utf-8";
        var html = new StringBuilder("<html><head><meta charset='utf-8'></head><body>");
        html.Append("<h1>Информация о клиенте</h1>");
        html.Append($"<p><b>IP адрес:</b> {context.Connection.RemoteIpAddress}</p>");
        html.Append($"<p><b>Браузер:</b> {context.Request.Headers["User-Agent"]}</p>");
        html.Append("<br><a href='/'>На главную</a></body></html>");
        await context.Response.WriteAsync(html.ToString());
    });
});

// 2. Таблицы (Запчасти, Поставщики и т.д.)
app.Map("/spareparts", b => b.Run(c => WriteTableResponse<SparePart>(c, "Запчасти", "SpareParts20")));
app.Map("/suppliers", b => b.Run(c => WriteTableResponse<Supplier>(c, "Поставщики", "Suppliers20")));
app.Map("/supplybatches", b => b.Run(c => WriteTableResponse<SupplyBatch>(c, "Поставки", "SupplyBatches20")));
app.Map("/supplyitems", b => b.Run(c => WriteTableResponse<SupplyItem>(c, "Позиции поставки", "SupplyItems20")));
app.Map("/sales", b => b.Run(c => WriteTableResponse<Sale>(c, "Продажи", "Sales20")));
app.Map("/saleitems", b => b.Run(c => WriteTableResponse<SaleItem>(c, "Позиции продажи", "SaleItems20")));
app.Map("/stocks", b => b.Run(c => WriteTableResponse<Stock>(c, "Остатки", "Stocks20")));

// 3. Поиск (Cookie)
app.Map("/searchform1", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var cookieVal = context.Request.Cookies["SearchFormData"];
        var model = !string.IsNullOrEmpty(cookieVal) ? JsonSerializer.Deserialize<SearchModel>(cookieVal) : new SearchModel();

        if (context.Request.Query.ContainsKey("query"))
        {
            model.QueryText = context.Request.Query["query"];
            model.Category = context.Request.Query["category"];
            model.SortOrder = context.Request.Query["sort"];
            context.Response.Cookies.Append("SearchFormData", JsonSerializer.Serialize(model), new CookieOptions { Expires = DateTime.Now.AddMinutes(5) });
        }
        await context.Response.WriteAsync(BuildSearchFormHtml("/searchform1", model, "Cookie"));
    });
});

// 4. Поиск (Session)
app.Map("/searchform2", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var model = context.Session.Get<SearchModel>("SearchFormData") ?? new SearchModel();

        if (context.Request.Query.ContainsKey("query"))
        {
            model.QueryText = context.Request.Query["query"];
            model.Category = context.Request.Query["category"];
            model.SortOrder = context.Request.Query["sort"];
            context.Session.Set("SearchFormData", model);
        }
        await context.Response.WriteAsync(BuildSearchFormHtml("/searchform2", model, "Session"));
    });
});

// --------------------------------------------------
// ФИНАЛЬНЫЙ ОБРАБОТЧИК (ГЛАВНАЯ + 404)
// --------------------------------------------------
app.Run(async context =>
{
    // Если путь - корень сайта "/", показываем МЕНЮ
    if (context.Request.Path == "/")
    {
        // Прогрев кэша
        var s = context.RequestServices;
        s.GetService<ICachedService<SparePart>>()?.AddEntitiesToCache("SpareParts20");
        s.GetService<ICachedService<Supplier>>()?.AddEntitiesToCache("Suppliers20");
        s.GetService<ICachedService<SupplyBatch>>()?.AddEntitiesToCache("SupplyBatches20");
        s.GetService<ICachedService<SupplyItem>>()?.AddEntitiesToCache("SupplyItems20");
        s.GetService<ICachedService<Sale>>()?.AddEntitiesToCache("Sales20");
        s.GetService<ICachedService<SaleItem>>()?.AddEntitiesToCache("SaleItems20");
        s.GetService<ICachedService<Stock>>()?.AddEntitiesToCache("Stocks20");

        context.Response.ContentType = "text/html;charset=utf-8";
        var sb = new StringBuilder("<html><head><meta charset='utf-8'><style>a {display:block; margin: 10px; font-size: 1.2em;} body{font-family: sans-serif; padding:20px;}</style></head><body>");
        sb.Append("<h1>Склад автозапчастей (Вариант 41)</h1>");
        sb.Append("<h3>Таблицы (Кэш: 266 сек):</h3>");
        sb.Append("<a href='/spareparts'>1. Запчасти</a>");
        sb.Append("<a href='/suppliers'>2. Поставщики</a>");
        sb.Append("<a href='/supplybatches'>3. Поставки</a>");
        sb.Append("<a href='/supplyitems'>4. Позиции поставки</a>");
        sb.Append("<a href='/sales'>5. Продажи</a>");
        sb.Append("<a href='/saleitems'>6. Позиции продажи</a>");
        sb.Append("<a href='/stocks'>7. Остатки</a>");
        sb.Append("<h3>Функции:</h3>");
        sb.Append("<a href='/info'>Информация о клиенте (/info)</a>");
        sb.Append("<a href='/searchform1'>Поиск (Cookie)</a>");
        sb.Append("<a href='/searchform2'>Поиск (Session)</a>");
        sb.Append("</body></html>");
        await context.Response.WriteAsync(sb.ToString());
    }
    // Если путь любой другой - ОШИБКА 404
    else
    {
        context.Response.StatusCode = 404;
        context.Response.ContentType = "text/html;charset=utf-8";
        await context.Response.WriteAsync("<h1>Page Not Found (Status 404)</h1><p>Проверьте адрес или перейдите <a href='/'>на главную</a>.</p>");
    }
});

app.Run();


// ==========================================
// ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
// ==========================================

async Task WriteTableResponse<T>(HttpContext context, string title, string cacheKey) where T : class
{
    var service = context.RequestServices.GetService<ICachedService<T>>();
    var data = service?.GetEntities(cacheKey);

    context.Response.ContentType = "text/html;charset=utf-8";
    var sb = new StringBuilder("<html><head><meta charset='utf-8'><style>table{border-collapse: collapse; width: 100%;} th, td {border: 1px solid black; padding: 8px;}</style></head><body>");
    sb.Append($"<h1>{title}</h1>");

    if (data != null && data.Any())
    {
        sb.Append("<table><thead><tr>");
        foreach (var prop in typeof(T).GetProperties()) sb.Append($"<th>{prop.Name}</th>");
        sb.Append("</tr></thead><tbody>");
        foreach (var item in data)
        {
            sb.Append("<tr>");
            foreach (var prop in typeof(T).GetProperties()) sb.Append($"<td>{prop.GetValue(item)}</td>");
            sb.Append("</tr>");
        }
        sb.Append("</tbody></table>");
    }
    else
    {
        sb.Append("<p>Нет данных в кэше. Кэш мог истечь или база пуста.</p>");
    }
    sb.Append("<br><a href='/'>На главную</a></body></html>");
    await context.Response.WriteAsync(sb.ToString());
}

string BuildSearchFormHtml(string actionUrl, SearchModel model, string storageType)
{
    return $@"
    <html><head><meta charset='utf-8'></head><body>
    <h1>Поиск запчастей ({storageType})</h1>
    <form action='{actionUrl}' method='GET'>
        <label>Текст запроса:</label><br>
        <input type='text' name='query' value='{model.QueryText}' /><br><br>
        <label>Категория:</label><br>
        <select name='category'>
            <option value='Part' {(model.Category == "Part" ? "selected" : "")}>Деталь</option>
            <option value='Manufacturer' {(model.Category == "Manufacturer" ? "selected" : "")}>Производитель</option>
        </select><br><br>
        <label>Сортировка:</label><br>
        <input type='radio' name='sort' value='Asc' {(model.SortOrder == "Asc" ? "checked" : "")}> А-Я
        <input type='radio' name='sort' value='Desc' {(model.SortOrder == "Desc" ? "checked" : "")}> Я-А<br><br>
        <input type='submit' value='Сохранить' />
    </form>
    <br><a href='/'>На главную</a>
    </body></html>";
}