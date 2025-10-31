public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/Product");

        group.MapGet("/{id}", GetById);
        // group.MapGet("/{name}", GetByName);

        return routes;
    }

    public static async Task<IResult> GetById(int id, ProductService service)
    {
        Product? product = await service.GetAsync(id);
        return product != null ? Results.Ok(product) : Results.NotFound();
    }
}
