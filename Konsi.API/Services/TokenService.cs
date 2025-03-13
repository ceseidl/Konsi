using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Konsi.API.Services
{
    public class TokenService
    {
        private readonly HttpClient _httpClient;

        public TokenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateTokenAsync(string username, string password)
        {
            // Monta o corpo da requisição
            var requestBody = new
            {
                username = username,
                password = password
            };

            // Realiza a chamada POST para gerar o token
            var response = await _httpClient.PostAsJsonAsync("/api/v1/token", requestBody);

            // Verifica se a resposta foi bem-sucedida
            response.EnsureSuccessStatusCode();

            // Deserializa o conteúdo da resposta para obter o token
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            return tokenResponse?.Token ?? string.Empty;
        }

        // Classe para mapear a resposta da API de token
        private class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}
