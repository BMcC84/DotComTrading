using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace DotComTrading.Tests
{
    //Integration Tests for WebsiteController using WebApplicationFactory
    public class WebsitesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;

        public WebsitesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();
        }

        //Tests that Get All Websites Endpoint Responds and Returns Ok
        [Fact]
        public async Task GetAllReturnsOk()
        {
            var response = await _httpClient.GetAsync("/api/websites");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //Tests that a 404 Error is returned when trying to access a website stock that does not exist
        [Fact]
        public async Task GetByIdReturnsNotFound()
        {
            var response = await _httpClient.GetAsync("/api/websites/0");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
