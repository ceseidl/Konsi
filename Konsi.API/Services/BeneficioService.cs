using Konsi.API.Models;
using System.Text.Json;

namespace Konsi.API.Services
{
    public class BeneficioService
    {
        private readonly HttpClient _httpClient;

        public BeneficioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Beneficio>> GetBenefitsAsync(string cpf)
        {
            var response = await _httpClient.GetAsync($"/api/v1/inss/consulta-beneficios?cpf={cpf}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Beneficio>>(content);
        }
    }
    

}
