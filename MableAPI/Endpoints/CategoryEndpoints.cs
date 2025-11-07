public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/Category");

        group.MapGet("/{id:int}", GetById).RequireAuthorization();
        group.MapDelete("/delete/{id}", DeleteById).RequireAuthorization();

        group.MapGet("/{name}", GetByName);
        // group.MapPost("/", CreateCategory);

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
}