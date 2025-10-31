using MableAPI.Data;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MableAPI.Tests;

public class UnitTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UnitTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
              builder.ConfigureServices(services =>
              {
                  services.AddDbContext<AppDbContext>(options =>
                  {
                      options.UseInMemoryDatabase("InMemoryDbForTesting");
                  });
              });
        });
    }

    [Fact]
    public async Task Test1()
    {
        var client = _factory.CreateClient();

        var response = await client.GetStringAsync("/");

        Assert.True(true);
    }
}
