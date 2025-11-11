using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MableAPI.Tests;
public class UnitTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public UnitTests(WebApplicationFactory<Program> factory)
    {
        client = factory
            .WithWebHostBuilder(b => b.UseEnvironment("Testing"))
            .CreateClient();
    }

    public async Task<string> GetTokenAsync()
    {
        var correctLogin = new { UserName = "username", Password = "password" };
        var response2 = await client.PostAsJsonAsync("/login", correctLogin);
        Assert.True(response2.IsSuccessStatusCode);

        var body = await response2.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString();
    }

    [Fact]
    public async Task TestLogin()
    {
        var wrongLogin = new { UserName = "wronguser", Password = "wrongpass" };        

        var response = await client.PostAsJsonAsync("/login", wrongLogin);
        Assert.False(response.IsSuccessStatusCode);

        string token = await GetTokenAsync();
        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public async Task TestAuthorization()
    {
        string token = await GetTokenAsync();

        var response = await client.PostAsJsonAsync("/Category/", new { Name = "TestCategory" });
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response2 = await client.PostAsJsonAsync("/Category/", new { Name = "TestCategory" });
        Assert.True(response2.IsSuccessStatusCode);
    }

    [Fact]
    public async Task TestCRUDCategory()
    {
        string token = await GetTokenAsync();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createResponse = await client.PostAsJsonAsync("/Category/", new { Name = "NewCategory" });
        Assert.True(createResponse.IsSuccessStatusCode);

        //getbyname
        var getResponse = await client.GetAsync("/Category/NewCategory");
        Assert.True(getResponse.IsSuccessStatusCode);

        var category = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("NewCategory", category.GetProperty("name").GetString());

        //getbyid
        var responseById = await client.GetAsync($"/Category/{category.GetProperty("id")}");
        Assert.True(responseById.IsSuccessStatusCode);

        //update
        var responseUpdate = await client.PutAsJsonAsync($"/Category/update/{category.GetProperty("id")}", new { Name = "UpdatedCategory" });
        Assert.True(responseUpdate.IsSuccessStatusCode);

        var updatedCategory = await responseUpdate.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("UpdatedCategory", updatedCategory.GetProperty("name").GetString());

        //delete
        var responseDelete = await client.DeleteAsync($"/Category/delete/{category.GetProperty("id")}");
        Assert.True(responseDelete.IsSuccessStatusCode);

        var responseGetDeleted = await client.GetAsync($"/Category/{category.GetProperty("id")}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, responseGetDeleted.StatusCode);
    }

    [Fact]
    public async Task TestCRUDProduct()
    {
        string token = await GetTokenAsync();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        Product newProduct = new Product
        {
            CategoryID = 1,
            Name = "TestProduct",
            DateAdded = DateTime.UtcNow,
            Price = 50.0,
            Discount = true,
            DiscountPrice = 45.0,
            IsFavorite = false
        };

        var createResponse = await client.PostAsJsonAsync("/Product/", newProduct);
        Assert.True(createResponse.IsSuccessStatusCode);

        //getbyname
        var getResponse = await client.GetAsync("/Product/TestProduct");
        Assert.True(getResponse.IsSuccessStatusCode);

        var products = await getResponse.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.Equal("TestProduct", products[0].GetProperty("name").GetString());

        //getbyid
        var responseById = await client.GetAsync($"/Product/{products[0].GetProperty("id")}");
        Assert.True(responseById.IsSuccessStatusCode);

        //update
        newProduct.Name = "UpdatedProduct";
        var responseUpdate = await client.PutAsJsonAsync($"/Product/update/{products[0].GetProperty("id")}", newProduct);
        Assert.True(responseUpdate.IsSuccessStatusCode);

        var updatedProduct = await responseUpdate.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("UpdatedProduct", updatedProduct.GetProperty("name").GetString());

        //delete
        var responseDelete = await client.DeleteAsync($"/Product/delete/{products[0].GetProperty("id")}");
        Assert.True(responseDelete.IsSuccessStatusCode);

        var responseGetDeleted = await client.GetAsync($"/Product/{products[0].GetProperty("id")}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, responseGetDeleted.StatusCode);
    }

    [Fact]
    public async Task TestFavoritesProduct()
    {
        string token = await GetTokenAsync();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        Product newProduct = new Product
        {
            CategoryID = 1,
            Name = "TestProduct",
            DateAdded = DateTime.UtcNow,
            Price = 50.0,
            Discount = true,
            DiscountPrice = 45.0,
            IsFavorite = false
        };

        var createResponse = await client.PostAsJsonAsync("/Product/", newProduct);
        Assert.True(createResponse.IsSuccessStatusCode);

        var responseFavorites = await client.GetAsync($"/Product/getFavoriteProducts/");
        var contentLength = responseFavorites.Content.Headers.ContentLength;
        //2 because it's [] as response
        Assert.True(contentLength == 2);

        var makeFavoriteResponse = await client.PostAsync($"/Product/makeFavorite/1", null);
        Assert.True(makeFavoriteResponse.IsSuccessStatusCode);

        var responseFavoritesAfter = await client.GetAsync($"/Product/getFavoriteProducts/");
        var favoriteProducts = await responseFavoritesAfter.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.True(favoriteProducts!.Count == 1);

        var removeFavoriteResponse = await client.PostAsync($"/Product/removeFavorite/1", null);
        Assert.True(removeFavoriteResponse.IsSuccessStatusCode);

        var responseFavoritesFinal = await client.GetAsync($"/Product/getFavoriteProducts/");
        var contentLengthFinal = responseFavoritesFinal.Content.Headers.ContentLength;
        Assert.True(contentLengthFinal == 2);
    }

    

}
