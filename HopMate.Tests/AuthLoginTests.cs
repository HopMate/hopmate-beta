using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hopmate.Server; // Altere se necessário para o namespace correto

namespace hopmate.Server.Tests
{
    [TestClass]
    public class AuthControllerTests
    {
        private readonly HttpClient _httpClient;

        public AuthControllerTests()
        {
            var appFactory = new WebApplicationFactory<Program>();
            _httpClient = appFactory.CreateDefaultClient();
        }

        [TestMethod]
        public async Task Login_Valido_DeveRetornarOk()
        {
            var loginData = new
            {
                Username = "zuca06",
                Password = "Tiagofer1402."
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginData);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task Login_Invalido_DeveRetornarUnauthorized()
        {
            var loginData = new
            {
                Username = "zuca06",
                Password = "senhaIncorreta123"
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginData);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Login_ComCamposVazios_DeveRetornarBadRequest()
        {
            var loginData = new
            {
                Username = "",
                Password = ""
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginData);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
