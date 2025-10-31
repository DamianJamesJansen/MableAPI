using Microsoft.EntityFrameworkCore;
using MableAPI.Data;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Needed for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add EF Core (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

var app = builder.Build();

// If in development, use Swagger. good for seeing the API calls available and their documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//insert the initial data into the database
using (var scope = app.Services.CreateScope())
{
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    string pathCategory = Path.Combine(AppContext.BaseDirectory, "Data", "catergories.json");
    string pathProduct = Path.Combine(AppContext.BaseDirectory, "Data", "articles.json");

    //if empty, insert from json
    if (File.Exists(pathCategory) && !db.Categories.Any())
    {
        string jsonCategories = await File.ReadAllTextAsync(pathCategory);
        List<Category>? categories = JsonSerializer.Deserialize<List<Category>>(jsonCategories);
        if (categories != null)
        {
            db.Categories.AddRange(categories);
        }
    }
    
    if (File.Exists(pathProduct) && !db.Products.Any())
    {        
        string jsonProducts = await File.ReadAllTextAsync(pathProduct);
        List<Product>? products = JsonSerializer.Deserialize<List<Product>>(jsonProducts);
        if (products != null)
        {
            //only add the products with a discount price of 100 or less
            db.Products.AddRange(products.Where(p => p.DiscountPrice <= 100));
        }
        db.SaveChanges();
    }
}

app.Run();
//needed for WebApplicationFactory in tests
public partial class Program { }
