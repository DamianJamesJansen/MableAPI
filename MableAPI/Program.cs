using Microsoft.EntityFrameworkCore;
using MableAPI.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection.Extensions;


var builder = WebApplication.CreateBuilder(args);
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));

// Needed for Swagger
builder.Services.AddEndpointsApiExplorer();

//authenticate button in swagger. not needed normally, but nice for testing now
//van internet geplukt, had zelf geen idee
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "You only need to paste the token, no Bearer prefix needed"
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// Add EF Core (SQLite)
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=app.db"));
}

//In production the secrets won't be stored in the appsettings.json, but only now for the demo. Normally a vault or secret store is used
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = key
        };
    });
builder.Services.AddAuthorization();

//these need to be added. these services are getting injected every time needed. which will be for calls to the endpoints
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

// If in development, use Swagger. good for seeing the API calls available and their documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//default .net template stuff
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

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

app.MapPost("/login", (LoginRequest req) =>
{
    bool isValidUser = req.Username == "username" && req.Password == "password";
    if (!isValidUser)
        return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, req.Username)
    };

    //chose an algorithm based on what visual studio suggested. I don't really know the differences to be honest
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: builder.Configuration["Jwt:Issuer"],
        audience: builder.Configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(60),
        signingCredentials: creds);

    return Results.Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token)
    });
});

app.MapCategoryEndpoints();
app.MapProductEndpoints();

app.Run();

//apparently a lightweight class that has an equals, hash and some other stuff implemented automatically
//new in .net 9. Learned this from this project :)
record LoginRequest(string Username, string Password);
//needed for WebApplicationFactory in tests
public partial class Program { }
