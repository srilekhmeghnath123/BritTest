using FunctionAppTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FunctionAppTest.Triggers
{
    public class ProductTrigger
    {
        private readonly ILogger<ProductTrigger> _logger;
        private readonly ApplicationDbContext _context;

        public ProductTrigger(ILogger<ProductTrigger> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetProduct")]
        public async Task<IActionResult> GetProduct([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(GetProduct)} trigger function processed a request.");
            var products = await _context.Products.ToListAsync();
            return new OkObjectResult(products);
        }

        [Function("AddProduct")]
        public async Task<IActionResult> AddProduct([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(AddProduct)} trigger function processed a request.");
            var newProduct = new Product
            {
                ProductName = req.Query["ProductName"],
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
            return new OkObjectResult(newProduct);
        }

        [Function("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(UpdateProduct)} trigger function processed a request.");
            int productId = int.Parse(req.Query["Id"]);
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                return new NotFoundResult();

            product.ProductName = req.Query["ProductName"];
            product.ModifiedBy = "Admin";
            product.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return new OkObjectResult(product);
        }

        [Function("RemoveProduct")]
        public async Task<IActionResult> RemoveProduct([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(RemoveProduct)} trigger function processed a request.");
            int productId = int.Parse(req.Query["Id"]);
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                return new NotFoundResult();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return new OkObjectResult("Product Removed");
        }
    }
}
