using FluentAssertions;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMockIntegrationTesting.service.Models;
using WireMockIntegrationTesting.service.Services;

namespace WireMockIntegrationTesting.TestingLab;
public class PlanetsServiceTests : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly WireMockServer _mockServer;
    private readonly PlanetService _planetService;
    

    public PlanetsServiceTests()
    {
        _mockServer = WireMockServer.Start(new WireMockServerSettings
        {
           Urls = new[]
           {
               "http://localhost:777/",
               "http://localhost:8888/",
               "http://localhost:9999/"
           },
           UseSSL = true
        });
        _httpClient = new HttpClient 
        { 
            BaseAddress = new Uri(_mockServer.Urls[0])
        };

            _planetService = new PlanetService( _httpClient );
    }
    public void Dispose()
    {
        _mockServer.Stop();
        _httpClient.Dispose();
    }

    [Fact]
    public async Task GivenThatPlanetExists_WhenGetPlanetByIdIsInvoked_ThenValidPlanetIsReturned()
    {
        //Arrange
        var planet = new Planet(4, "Mars", 6779, 2, true);

        _mockServer
            .Given(
               Request.Create()
               .UsingGet()
               .WithPath("/planets/4")

            )
            .RespondWith(
              Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(planet)
            );

        //Act 
        var result = await _planetService.GetPlanetByIdAsync(planet.Id);

        //Assert

        result.Should().NotBeNull();
        result.Name.Should().Be(planet.Name);
        result.Diameter.Should().Be(planet.Diameter);
        result.NumberOfMoons.Should().Be(planet.NumberOfMoons);
        result.HasAtmosphere.Should().Be(planet.HasAtmosphere);
    }


    [Fact]
    public async Task GivenThatPlanetDoesntExists_WhenGetPlanetIsInvoked_ThenNullIsReturnedd()
    {
        //Arrange
        _mockServer
            .Given(
              Request.Create()
              .UsingGet()
              .WithPath("/planets/9"))
            .RespondWith(
              Response.Create()
              .WithStatusCode(HttpStatusCode.NotFound)
              .WithDelay(TimeSpan.FromMilliseconds(500))
            );

        //Act 
        var result = await _planetService.GetPlanetByIdAsync(9);

        //Assert
        result.Should().BeNull();
    }
}

