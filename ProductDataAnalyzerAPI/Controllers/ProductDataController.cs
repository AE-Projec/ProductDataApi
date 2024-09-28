using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductDataAnalyzerAPI.Services;
using System.Text.Json;

namespace ProductDataAnalyzerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDataController : ControllerBase
    {
        private readonly ProductDataService _productDataService;

        public ProductDataController(ProductDataService productDataService)
        {
            _productDataService = productDataService;
        }


        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productDataService.GetProductsDataAsync();
            return Ok(products);
        }

        [HttpGet("most-expensive")]
        public async Task<IActionResult> GetMostExpensiveBeer([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var beers = await _productDataService.GetProductsDataAsync();
            var mostExpensiveBeer = beers
                .SelectMany(b => b.Articles)
                .OrderByDescending(a => a.Price / 0.5m)
                .FirstOrDefault();

            return Ok(mostExpensiveBeer);
        }

        [HttpGet("cheapest")]
        public async Task<IActionResult> GetCheapestBeer([FromQuery] string url)
        {
            var beers = await _productDataService.GetProductsDataAsync();
            var chepestBeer = beers
                .SelectMany(b => b.Articles)
                .OrderBy(a => a.Price / 0.5m)
                .FirstOrDefault();

            return Ok(chepestBeer);
        }

        [HttpGet("exact-price")]
        public async Task<IActionResult> GetExactPrice([FromQuery] string url)
        {
            var beers = await _productDataService.GetProductsDataAsync();
            var exactPriceBeer = beers
                .SelectMany(b => b.Articles)
                .Where(a => a.Price == 17.99m)
                .ToList();

            return Ok(exactPriceBeer);
        }

        [HttpGet("most-bottles")]
        public async Task<IActionResult> GetMostBottles([FromQuery] string url)
        {
            var bottles = await _productDataService.GetProductsDataAsync();
            var mostBottles = bottles
                .SelectMany(b => b.Articles)
                .Select(a => new
                {
                    ProductName = a.ShortDescription,
                    BottleCount = ExtractBottleCount(a.ShortDescription)
                })
                .OrderByDescending(a => a.BottleCount)
                .FirstOrDefault();

            return Ok(mostBottles);
        }

        [HttpGet("all-routes")]
        public async Task<IActionResult> GetAllRoutes()
        {
            var allBeers = await _productDataService.GetProductsDataAsync();

            var mostExpensiveBeer = allBeers
                .SelectMany(b => b.Articles)
                .OrderByDescending(a => a.Price / 0.5m)
                .FirstOrDefault();

            var cheapestBeer = allBeers
                .SelectMany(b => b.Articles)
                .OrderBy(a => a.Price / 0.50m)
                .FirstOrDefault();

            var exactPriceBeer = allBeers
                .SelectMany(b => b.Articles)
                .Where(a => a.Price == 17.99m)
                .ToList();

            var productWithMostBottles = allBeers
                .SelectMany(b => b.Articles)
                .Select(a => new
                {
                    ProductName = a.ShortDescription,
                    BootleCount = ExtractBottleCount(a.ShortDescription)
                })
                .OrderByDescending(a => a.BootleCount)
                .FirstOrDefault();

            var allAnswers = new
            {
                MostExpensiveBeer = mostExpensiveBeer,
                CheapestBeer = cheapestBeer,
                ExactPriceBeer = exactPriceBeer,
                ProductWithMostBottles = productWithMostBottles
            };

            return Ok(allAnswers);
        }


        //hilfsmethode um die anzahl der flaschen aus dem string zu extrahieren
        private int ExtractBottleCount(string description)
        {
            var parts = description.Split('x');
            if (parts.Length > 0 && int.TryParse(parts[0].Trim(),out var bottleCount))
            {
                return bottleCount;
            }
            return 0;
        }
    }
}
