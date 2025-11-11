using System.Text.RegularExpressions;
using MableAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualBasic;

public class ProductService
{
    private readonly AppDbContext dbContext;
    public ProductService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Product?> GetAsync(int id)
    {
        return await dbContext.Products.FindAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        Product? product = await dbContext.Products.FindAsync(id);
        if (product != null)
        {
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<Product>> GetByNameAsync(string name)
    {
        return await dbContext.Products
            .Where(p => p.Name.ToLower() == name.ToLower())
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(Product product)
    {
        //could have done the datenow instead of the date being passed in
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Product?> UpdateProduct(int id, Product updatedProduct)
    {
        // check if exists
        Product? existingProduct = await GetAsync(id);
        if (existingProduct == null)
            return null;

        existingProduct.CategoryID = updatedProduct.CategoryID;
        existingProduct.Name = updatedProduct.Name;
        existingProduct.DateAdded = updatedProduct.DateAdded;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.Discount = updatedProduct.Discount;
        existingProduct.DiscountPrice = updatedProduct.DiscountPrice;

        dbContext.Products.Update(existingProduct);
        await dbContext.SaveChangesAsync();
        return existingProduct;
    }

    public async Task<Dictionary<string, List<Product>>> GetProductGroupedByCategoryAsync()
    {
        var result = await (
            from p in dbContext.Products
            join c in dbContext.Categories on p.CategoryID equals c.Id
            group p by c.Name into g
            select new { CategoryName = g.Key, Products = g.ToList() }
        ).ToDictionaryAsync(x => x.CategoryName, x => x.Products);
        return result;
    }

    public async Task<IResult> MakeFavorite(int productId)
    {
        Product? product = await dbContext.Products.FindAsync(productId);
        if (product != null)
        {
            product.IsFavorite = true;
            dbContext.Products.Update(product);
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        }
        return Results.NotFound();
    }

    public async Task<IResult> RemoveFavorite(int productId)
    {
        Product? product = await dbContext.Products.FindAsync(productId);
        if (product != null)
        {
            product.IsFavorite = false;
            dbContext.Products.Update(product);
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        }
        return Results.NotFound();
    }

    public async Task<List<Product>> GetFavoritesAsync()
    {
        return await dbContext.Products.Where(p => p.IsFavorite).ToListAsync();
    }
}