public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/Product");

        group.MapGet("/{id:int}", GetById).RequireAuthorization();
        group.MapDelete("/delete/{id}", DeleteById).RequireAuthorization();

        group.MapGet("/{name}", GetByName);

        return routes;
    }

    public static async Task<IResult> GetById(int id, ProductService service)
    {
        Product? product = await service.GetAsync(id);
        return product != null ? Results.Ok(product) : Results.NotFound();
    }

    public static async Task<IResult> DeleteById(int id, ProductService service)
    {
        await service.DeleteAsync(id);
        return Results.Ok();
    }

    public static async Task<IResult> GetByName(string name, ProductService service)
    {
        List<Product> product = await service.GetByNameAsync(name);
        return product != null ? Results.Ok(product.ToString) : Results.NotFound();
    }
}
