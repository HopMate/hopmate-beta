using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hopmate.Server;
using System;

namespace hopmate.Server.Tests
{
    [TestClass]
    public class AuthRegisterTests
    {
        private readonly HttpClient _httpClient;
        public AuthRegisterTests()
        {
            var factory = new WebApplicationFactory<Program>();
            _httpClient = factory.CreateDefaultClient();
        }

        [TestMethod]
        public async Task Registo_Valido_DeveRetornarOk()
        {
            var random = Guid.NewGuid().ToString("N").Substring(0, 8);

            var userData = new
            {
                Name = "Test User",
                Username = $"testuser_{random}",
                Email = $"test_{random}@example.com",
                Password = "Tiagofer1402.#",
                DateOfBirth = "2000-01-01",
                HasDrivingLicense = true
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", userData);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task Registo_CamposEmBranco_DeveRetornarBadRequest()
        {
            var userData = new
            {
                Name = "",
                Username = "",
                Email = "",
                Password = "",
                DateOfBirth = "2000-01-01",
                HasDrivingLicense = false
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", userData);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task Registo_PasswordFraca_DeveRetornarBadRequest()
        {
            var userData = new
            {
                Name = "Weak Password User",
                Username = $"weakpass_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                Email = $"weak{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                Password = "123",
                DateOfBirth = "1990-01-01",
                HasDrivingLicense = true
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", userData);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}