using MableAPI.Data;
using Microsoft.EntityFrameworkCore;

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
        var products = dbContext.Products
            .Where(p => p.Name.ToLower() == name.ToLower())
            .ToListAsync();
        return await products;
    }
}