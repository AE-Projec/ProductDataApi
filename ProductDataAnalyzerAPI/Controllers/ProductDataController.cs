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

        [HttpGet("most-expensive-and-cheapest")]
        public async Task<IActionResult> GetMostExpensiveAndCheapestBeer([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var beers = await _productDataService.GetProductsDataAsync(url);

            var beerPrices = beers
                .SelectMany(b => b.Articles.Select(a => new
                {
                    ProductName = b.Name,
                    BrandName = b.BrandName,
                    PricePerLitre = ExtractPricePerLitre(a.PricePerUnitText),
                    PricePerUnitText = a.PricePerUnitText
                }))
                .OrderBy(a => a.PricePerLitre)
                .ToList();

            var cheapestBeer = beerPrices.FirstOrDefault();
            var mostExpensiveBeer = beerPrices.LastOrDefault();

            return Ok(new
            {
                CheapestBeer = new
                {
                    cheapestBeer?.ProductName,
                    cheapestBeer?.BrandName,
                    cheapestBeer?.PricePerUnitText
                },
                MostExpensiveBeer = new
                {
                    mostExpensiveBeer?.ProductName,
                    mostExpensiveBeer?.BrandName,
                    mostExpensiveBeer?.PricePerUnitText
                }
            });
        }

        [HttpGet("exact-price")]
        public async Task<IActionResult> GetExactPrice([FromQuery] string url)
        {
            var beers = await _productDataService.GetProductsDataAsync(url);
            var exactBeerPrice = beers
                .SelectMany(b => b.Articles.Select(a => new
                {
                    ProductPrice = a.Price,
                    ProductName = b.Name,
                    ProductDiscription = a.ShortDescription,
                    PricePerLitre = ExtractPricePerLitre(a.PricePerUnitText),
                    PricePerUnitText = a.PricePerUnitText

                }))
                .Where(a => a.ProductPrice == 17.99m)
                .OrderBy(a => a.PricePerLitre)
                .ToList();
            
            return Ok(new
            {
                ExactBeerPrice = exactBeerPrice.Select(b=>  new
                {
                    b.ProductPrice,
                    b.ProductName,
                    b.ProductDiscription,
                    b.PricePerUnitText
                })
            });
        }

        [HttpGet("most-bottles")]
        public async Task<IActionResult> GetMostBottles([FromQuery] string url, bool returnAll = false)
        {
            var bottles = await _productDataService.GetProductsDataAsync(url);

            var mostBottles = bottles
                .SelectMany(b => b.Articles)
                .Select(a => ExtractBottleCount(a.ShortDescription))
                .Max();

            if (returnAll)
            {
                var mostBottlesAllProducts = bottles
                    .SelectMany(b => b.Articles.Select(a => new
                    {
                        ProductName = b.Name,
                        ProductDescription = a.ShortDescription,
                        BottleCount = ExtractBottleCount(a.ShortDescription)
                    }))
                    .Where(a => a.BottleCount == mostBottles)
                    .ToList();

                return Ok(mostBottlesAllProducts);
            }
            else
            {
                var mostBottlesSingleProduct = bottles
                    .SelectMany(b => b.Articles.Select(a => new
                    {
                        ProductName = b.Name,
                        ProductDescription = a.ShortDescription,
                        BottleCount = ExtractBottleCount(a.ShortDescription)
                    }))
                    .OrderByDescending(a => a.BottleCount)
                    .FirstOrDefault();

                return Ok(mostBottlesSingleProduct);
            }
        }

        [HttpGet("all-routes")]
        public async Task<IActionResult> GetAllRoutes([FromQuery] string url)
        {
            var allBeers = await _productDataService.GetProductsDataAsync(url);

            var beerPrices = allBeers
                 .SelectMany(b => b.Articles.Select(a => new
                 {
                     ProductName = b.Name,
                     BrandName = b.BrandName,
                     PricePerLitre = ExtractPricePerLitre(a.PricePerUnitText),
                     PricePerUnitText = a.PricePerUnitText
                 }))
                 .OrderBy(a => a.PricePerLitre)
                 .ToList();

            var cheapestBeer = beerPrices.FirstOrDefault();
            var mostExpensiveBeer = beerPrices.LastOrDefault();


            var exactPriceBeer = allBeers
                .SelectMany(b => b.Articles)
                .Where(a => a.Price == 17.99m)
                .Select(a => new
                {
                    ProductName = a.ShortDescription,
                    Price = a.Price,
                    PricePerUnitText = a.PricePerUnitText
                })
                .OrderBy(a => ExtractPricePerLitre(a.PricePerUnitText))
                .ToList();


            var maxBottleCount = allBeers
                .SelectMany(b => b.Articles)
                .Select(a => ExtractBottleCount(a.ShortDescription))
                .Max();

            var productWithMostBottles = allBeers
                .SelectMany(b => b.Articles.Select(a => new
                
                
                {
                    ProductName = b.Name,
                    ProductDiscription = a.ShortDescription,
                    BottleCount = ExtractBottleCount(a.ShortDescription)
                }))
                .Where(a => a.BottleCount == maxBottleCount)
                .ToList();


            var allAnswers = new
            {

                CheapestBeer = new
                {
                    cheapestBeer?.ProductName,
                    cheapestBeer?.BrandName,
                    cheapestBeer?.PricePerUnitText
                },
                MostExpensiveBeer = new
                {
                    mostExpensiveBeer?.ProductName,
                    mostExpensiveBeer?.BrandName,
                    mostExpensiveBeer?.PricePerUnitText
                },
                ExactPriceBeer = exactPriceBeer,
                ProductWithMostBottles = productWithMostBottles
            };

            return Ok(allAnswers);
        }


        private int ExtractBottleCount(string description)
        {
            var parts = description.Split('x');
            if (parts.Length > 0 && int.TryParse(parts[0].Trim(), out var bottleCount))
            {
                return bottleCount;
            }
            return 0;
        }


        private decimal ExtractPricePerLitre(string pricePerUnitText)
        {
            var cleanedText = pricePerUnitText
                .Replace("€/Liter", "")
                .Replace("(", "")
                .Replace(")", "")
                .Trim();

            cleanedText = cleanedText.Replace(",", ".");

            if (decimal.TryParse(cleanedText, out var pricePerLitre))
            {
                return pricePerLitre;

            }

            return 0;
        }

    }
}
