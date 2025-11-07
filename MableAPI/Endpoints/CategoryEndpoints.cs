public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/Category");

        group.MapGet("/{id}", GetById).RequireAuthorization();
        group.MapDelete("/{id}", DeleteById).RequireAuthorization();
        // group.MapGet("/{name}", GetByName);
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
}