using MableAPI.Data;

public class CategoryService
{
    private readonly AppDbContext dbContext;
    public CategoryService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Category?> GetAsync(int id)
    {
        return await dbContext.Categories.FindAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        Category? category = await dbContext.Categories.FindAsync(id);
        if (category != null)
        {
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
        }
    }
}