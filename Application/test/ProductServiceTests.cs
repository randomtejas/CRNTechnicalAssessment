using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SDTechnicalAssessment.Application.DTOs;
using SDTechnicalAssessment.Application.Services;
using SDTechnicalAssessment.Domain.Entities;
using SDTechnicalAssessment.Infrastructure.Data.Repositories;
using Xunit;

namespace SDTechnicalAssessment.Application.Tests
{
    // ------------------------------------------------------------
    // This class contains unit tests for ProductService.
    //
    // We are testing the service without connecting to the
    // real SQL Server database.
    // ------------------------------------------------------------
    public class ProductServiceTests
    {
        // --------------------------------------------------------
        // Test: UpdateAsync should return true when the product
        // exists in the database.
        // --------------------------------------------------------
        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenProductExists()
        {
            // ----------------------------------------------------
            // Arrange
            //
            // Arrange means preparing everything required
            // before executing the method we want to test.
            // ----------------------------------------------------

            // Create a fake ProductRepository using Moq.
            var repositoryMock = new Mock<IProductRepository>();

            // Create a sample product.
            var product = new Product
            {
                Id = 1,
                ProductName = "Old Product",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow
            };

            // Tell the mock repository:
            //
            // "When GetByIdAsync(1) is called,
            // return this product."
            repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Create the service.
            //
            // NullLogger is used because logging is not what
            // this particular test is trying to verify.
            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            // Data that we want to update.
            var updateDto = new UpdateProductDto
            {
                ProductName = "Updated Product",
                ModifiedBy = "admin"
            };

            // ----------------------------------------------------
            // Act
            //
            // Act means executing the actual method being tested.
            // ----------------------------------------------------
            var result = await service.UpdateAsync(1, updateDto);

            // ----------------------------------------------------
            // Assert
            //
            // Assert means checking whether the result is what
            // we expected.
            // ----------------------------------------------------

            // UpdateAsync should return true when product exists.
            Assert.True(result);

            // Verify that the product name was actually changed.
            Assert.Equal("Updated Product", product.ProductName);

            // Verify that ModifiedBy was updated.
            Assert.Equal("admin", product.ModifiedBy);

            // Verify that repository UpdateAsync was called once.
            repositoryMock.Verify(
                x => x.UpdateAsync(product),
                Times.Once
            );
        }



        // ------------------------------------------------------------
        // Test: UpdateAsync should return false when the product
        // does not exist in the database.
        // ------------------------------------------------------------
        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            // --------------------------------------------------------
            // Arrange
            // --------------------------------------------------------

            // Create a fake ProductRepository.
            var repositoryMock = new Mock<IProductRepository>();

            // Tell the mock repository:
            //
            // When ProductService asks for Product ID 999,
            // pretend that the product does not exist.
            repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Create ProductService using our mock repository.
            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            // Create update data.
            var updateDto = new UpdateProductDto
            {
                ProductName = "Updated Product",
                ModifiedBy = "admin"
            };

            // --------------------------------------------------------
            // Act
            // --------------------------------------------------------

            // Try to update Product ID 999.
            var result = await service.UpdateAsync(999, updateDto);

            // --------------------------------------------------------
            // Assert
            // --------------------------------------------------------

            // Since the product doesn't exist,
            // UpdateAsync should return false.
            Assert.False(result);

            // UpdateAsync should NOT call the repository's UpdateAsync
            // because there is no product to update.
            repositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Product>()),
                Times.Never
            );
        }



        // ------------------------------------------------------------
        // Test: DeleteAsync should return true when the product exists.
        // ------------------------------------------------------------
        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenProductExists()
        {
            // --------------------------------------------------------
            // Arrange
            // --------------------------------------------------------

            // Create a fake ProductRepository.
            var repositoryMock = new Mock<IProductRepository>();

            // Create a sample product.
            var product = new Product
            {
                Id = 1,
                ProductName = "Test Product",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow
            };

            // Tell the mock repository that Product ID 1 exists.
            repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Create ProductService with the mock repository.
            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            // --------------------------------------------------------
            // Act
            // --------------------------------------------------------

            // Try to delete Product ID 1.
            var result = await service.DeleteAsync(1);

            // --------------------------------------------------------
            // Assert
            // --------------------------------------------------------

            // Product exists, so DeleteAsync should return true.
            Assert.True(result);

            // Verify that DeleteAsync was called exactly once.
            repositoryMock.Verify(
                x => x.DeleteAsync(product),
                Times.Once
            );
        }




        // ------------------------------------------------------------
        // Test: DeleteAsync should return false when the product
        // does not exist.
        // ------------------------------------------------------------
        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            // --------------------------------------------------------
            // Arrange
            // --------------------------------------------------------

            // Create a fake ProductRepository.
            var repositoryMock = new Mock<IProductRepository>();

            // Tell the mock repository:
            // Product ID 999 does not exist.
            repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Create ProductService with the mock repository.
            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            // --------------------------------------------------------
            // Act
            // --------------------------------------------------------

            // Try to delete a product that does not exist.
            var result = await service.DeleteAsync(999);

            // --------------------------------------------------------
            // Assert
            // --------------------------------------------------------

            // Since the product doesn't exist,
            // DeleteAsync should return false.
            Assert.False(result);

            // Make sure the repository delete method
            // was never called.
            repositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<Product>()),
                Times.Never
            );
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();

            var product = new Product
            {
                Id = 1,
                ProductName = "Test Product",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow
            };

            repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Product", result.ProductName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();

            repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedProducts()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();

            var products = new List<Product>
    {
        new Product
        {
            Id = 1,
            ProductName = "Product 1",
            CreatedBy = "admin",
            CreatedOn = DateTime.UtcNow
        },
        new Product
        {
            Id = 2,
            ProductName = "Product 2",
            CreatedBy = "admin",
            CreatedOn = DateTime.UtcNow
        }
    };

            repositoryMock
                .Setup(x => x.GetAllAsync(1, 2))
                .ReturnsAsync((products, 5));

            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            // Act
            var result = await service.GetAllAsync(1, 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(5, result.TotalRecords);
            Assert.Equal(3, result.TotalPages);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedProduct()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();

            var product = new Product
            {
                Id = 1,
                ProductName = "New Product",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow
            };

            repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync(product);

            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            var createDto = new CreateProductDto
            {
                ProductName = "New Product",
                CreatedBy = "admin"
            };

            // Act
            var result = await service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Product", result.ProductName);
            Assert.Equal("admin", result.CreatedBy);

            repositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Product>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldMapDtoToProductCorrectly()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();

            Product? capturedProduct = null;

            repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(product => capturedProduct = product)
                .ReturnsAsync((Product product) => product);

            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            var createDto = new CreateProductDto
            {
                ProductName = "Mapped Product",
                CreatedBy = "admin"
            };

            // Act
            await service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(capturedProduct);
            Assert.Equal("Mapped Product", capturedProduct.ProductName);
            Assert.Equal("admin", capturedProduct.CreatedBy);
            Assert.NotEqual(default, capturedProduct.CreatedOn);

            repositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Product>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectSecondPage()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();

            var products = new List<Product>
    {
        new Product
        {
            Id = 3,
            ProductName = "Product 3",
            CreatedBy = "admin",
            CreatedOn = DateTime.UtcNow
        },
        new Product
        {
            Id = 4,
            ProductName = "Product 4",
            CreatedBy = "admin",
            CreatedOn = DateTime.UtcNow
        }
    };

            repositoryMock
                .Setup(x => x.GetAllAsync(2, 2))
                .ReturnsAsync((products, 5));

            var service = new ProductService(
                repositoryMock.Object,
                NullLogger<ProductService>.Instance
            );

            // Act
            var result = await service.GetAllAsync(2, 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.PageNumber);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(5, result.TotalRecords);
            Assert.Equal(3, result.TotalPages);

            Assert.Equal(2, result.Data.Count);
            Assert.Equal(3, result.Data[0].Id);
            Assert.Equal("Product 3", result.Data[0].ProductName);
        }
    }
}