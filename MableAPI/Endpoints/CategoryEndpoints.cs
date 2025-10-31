public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/Category");

        group.MapGet("/{id}", GetById);
        // group.MapGet("/{name}", GetByName);
        // group.MapPost("/", CreateCategory);

        return routes;
    }

    private static async Task<IResult> GetById(int id, CategoryService service)
    {
        Category? category = await service.GetAsync(id);
        return category != null ? Results.Ok(category) : Results.NotFound();
    }
}