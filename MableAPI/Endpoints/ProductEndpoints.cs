public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/Product");

        group.MapGet("/{id:int}", GetById).RequireAuthorization();
        group.MapDelete("/delete/{id}", DeleteById).RequireAuthorization();

        group.MapGet("/{name}", GetByName).RequireAuthorization();
        group.MapPost("/", CreateProduct).RequireAuthorization();
        group.MapPut("/update/{id}", UpdateProduct).RequireAuthorization();
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
        List<Product> products = await service.GetByNameAsync(name);
        return products != null && products.Count > 0 ? Results.Json(products) : Results.NotFound();
    }

    public static async Task<IResult> CreateProduct(Product product, ProductService service)
    {
        await service.CreateAsync(product);
        return Results.Created($"/Product/{product.Id}", product);
    }

    public static async Task<IResult> UpdateProduct(int id, Product updatedProduct, ProductService service)
    {
        Product? updatedEntry = await service.UpdateProduct(id, updatedProduct);
        return updatedEntry != null ? Results.Ok(updatedEntry) : Results.NotFound();
    }
}
