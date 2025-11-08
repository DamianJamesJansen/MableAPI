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
    public async Task TestCreateAndGetCategory()
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
    }

    [Fact]
    public async Task TestCreateAndGetProduct()
    {
        string token = await GetTokenAsync();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createResponse = await client.PostAsJsonAsync("/Product/", new
        {
            CategoryId = 1,
            Name = "NewProduct",
            DateAdded = DateTime.UtcNow,
            Price = 100.0,
            Discount = 10.0,
            DiscountPrice = 90.0
        });
        Assert.True(createResponse.IsSuccessStatusCode);

        //getbyname
        var getResponse = await client.GetAsync("/Product/NewProduct");
        Assert.True(getResponse.IsSuccessStatusCode);

        var products = await getResponse.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.Equal("NewProduct", products[0].GetProperty("name").GetString());

        //getbyid
        var responseById = await client.GetAsync($"/Product/{products[0].GetProperty("id")}");
        Assert.True(responseById.IsSuccessStatusCode);
    }

}
