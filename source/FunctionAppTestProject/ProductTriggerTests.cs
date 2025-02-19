using Moq;
using Microsoft.EntityFrameworkCore;
using FunctionAppTest.Models;
using FunctionAppTest.Triggers;
using Xunit;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace FunctionAppTestProject
{
    public class ProductTriggerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly ProductTrigger _productTrigger;

        public ProductTriggerTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _productTrigger = new ProductTrigger(Mock.Of<ILogger<ProductTrigger>>(), _mockContext.Object);
        }

        [Fact]
        public async Task GetProduct_Returns_Products()
        {
            // Arrange
            var mockSet = new Mock<DbSet<Product>>();
            _mockContext.Setup(c => c.Products).Returns(mockSet.Object);

            // Act
            var result = await _productTrigger.GetProduct(Mock.Of<HttpRequest>());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}