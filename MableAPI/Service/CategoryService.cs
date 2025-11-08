using MableAPI.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Category?> GetByNameAsync(string name)
    {
        // for some reason it doesn't like equals. weird
        return await dbContext.Categories.FirstAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> CreateAsync(Category category)
    {
        //check if already exists
        if (await dbContext.Categories.AnyAsync(c => c.Name.ToLower() == category.Name.ToLower()))
            return false;
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Category?> UpdateCategory(int id, Category updatedCategory)
    {
        // check if exists
        Category? existingCategory = await GetAsync(id);
        if (existingCategory == null)
            return null;

        existingCategory.Name = updatedCategory.Name;
        dbContext.Categories.Update(existingCategory);
        await dbContext.SaveChangesAsync();
        return existingCategory;
    }
}