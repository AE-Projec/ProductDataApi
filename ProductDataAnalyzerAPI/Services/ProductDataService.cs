using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

namespace ProductDataAnalyzerAPI.Services
{
    public class ProductDataService
    {
        private readonly HttpClient _httpClient;

        public ProductDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Products>> GetProductsDataAsync()
        {
            string url = "https://flapotest.blob.core.windows.net/test/ProductData.json";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<Products>>(jsonData);

                return products;
            }
            else
            {
                throw new HttpRequestException("Fehler beim aufrufen der Daten");
            }

        }
    }
}
