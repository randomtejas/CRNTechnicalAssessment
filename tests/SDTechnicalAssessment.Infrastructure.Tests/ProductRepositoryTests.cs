using Microsoft.EntityFrameworkCore;
using SDTechnicalAssessment.Domain.Entities;
using SDTechnicalAssessment.Infrastructure.Data;
using SDTechnicalAssessment.Infrastructure.Data.Repositories;

namespace SDTechnicalAssessment.Infrastructure.Tests
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedProducts()
        {
            // Arrange
            // Create an in-memory database for this test.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var context = new ApplicationDbContext(options);

            // Add test products to the in-memory database.
            context.Products.AddRange(
                new Product
                {
                    ProductName = "Product 1",
                    CreatedBy = "admin",
                    CreatedOn = DateTime.UtcNow
                },
                new Product
                {
                    ProductName = "Product 2",
                    CreatedBy = "admin",
                    CreatedOn = DateTime.UtcNow
                },
                new Product
                {
                    ProductName = "Product 3",
                    CreatedBy = "admin",
                    CreatedOn = DateTime.UtcNow
                });

            await context.SaveChangesAsync();

            // Create the repository using our test database.
            var repository = new ProductRepository(context);

            // Act
            // Request page 1 with 2 products per page.
            var result = await repository.GetAllAsync(1, 2);

            // Assert
            Assert.Equal(2, result.Products.Count);
            Assert.Equal(3, result.TotalRecords);
        }
    }
}