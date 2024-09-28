using System.Text.Json.Serialization;

namespace ProductDataAnalyzerAPI
{
    public class Products
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("brandName")]
        public string? BrandName { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("descriptionText")]
        public string? Description { get; set; }

        [JsonPropertyName("articles")]
        public List<Articles>? Articles { get; set; }
    }


}
