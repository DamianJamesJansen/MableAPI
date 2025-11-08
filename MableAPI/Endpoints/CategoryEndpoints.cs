public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/Category");

        group.MapGet("/{id:int}", GetById).RequireAuthorization();
        group.MapDelete("/delete/{id}", DeleteById).RequireAuthorization();

        group.MapGet("/{name}", GetByName).RequireAuthorization();
        group.MapPost("/", CreateCategory).RequireAuthorization();
        group.MapPut("/update/{id}", UpdateCategory).RequireAuthorization();
        return routes;
    }

    private static async Task<IResult> GetById(int id, CategoryService service)
    {
        Category? category = await service.GetAsync(id);
        return category != null ? Results.Ok(category) : Results.NotFound();
    }

    private static async Task<IResult> DeleteById(int id, CategoryService service)
    {
        await service.DeleteAsync(id);
        return Results.Ok();
    }

    private static async Task<IResult> GetByName(string name, CategoryService service)
    {
        Category? category = await service.GetByNameAsync(name);
        return category != null ? Results.Ok(category) : Results.NotFound();
    }

    private static async Task<IResult> CreateCategory(Category category, CategoryService service)
    {
        bool isValid = await service.CreateAsync(category);
        return isValid ? Results.Created($"/Category/{category.Id}", category) : Results.Conflict(new { message = "Category with the same name already exists." });
    }

    private static async Task<IResult> UpdateCategory(int id, Category updatedCategory, CategoryService service)
    {
        Category? updatedEntry = await service.UpdateCategory(id, updatedCategory);
        return updatedEntry != null ? Results.Ok(updatedEntry) : Results.NotFound();
    }
}