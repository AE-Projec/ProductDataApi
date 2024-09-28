using System.Text.Json.Serialization;

namespace ProductDataAnalyzerAPI
{
    public class Articles
    {
        [JsonPropertyName("id")]
        public int? ArticleId { get; set; }

        [JsonPropertyName("shortDescription")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ShortDescription { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

        [JsonPropertyName("pricePerUnitText")]
        public string? PricePerUnitText { get; set; }

        [JsonPropertyName("image")]
        public string? ImageUrl { get; set; }
    }
}
