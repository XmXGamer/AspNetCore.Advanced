using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCore.Advanced.SecretAuthentication;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SecretAuthentication.ExampleApp;
using Xunit;

namespace SecretAuthentication.Tests
{
    public class EmptySecretAuthenticationIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;

        public EmptySecretAuthenticationIntegrationTests(WebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task ShouldBe401_IfNoSecretIsInRequest()
        {
            static void ConfigureTestServices(IServiceCollection services) =>
                services.AddAuthentication(SecretAuthenticationDefaults.EmptyAuthenticationSchema).AddEmptySchemaSecretAuthentication(o => o.Secret = Guid.NewGuid().ToString());
            var client = _webApplicationFactory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(ConfigureTestServices);
                })
                .CreateClient();

            var response = await client.GetAsync("WeatherForecast");

            response.Should().Be401Unauthorized();
        }

        [Fact]
        public async Task ShouldBe200_IfTheRightSecretisInRequest()
        {
            var testSecret = Guid.NewGuid().ToString();
            void ConfigureTestServices(IServiceCollection services) =>
                services.AddAuthentication(SecretAuthenticationDefaults.EmptyAuthenticationSchema).AddEmptySchemaSecretAuthentication(o => o.Secret = testSecret);
            var client = _webApplicationFactory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(ConfigureTestServices);
                })
                .CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", testSecret);
            var response = await client.GetAsync("WeatherForecast");

            response.Should().Be200Ok();
        }
    }
}