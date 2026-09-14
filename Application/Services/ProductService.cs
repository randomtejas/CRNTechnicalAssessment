using SDTechnicalAssessment.Application.DTOs;
using SDTechnicalAssessment.Application.Interfaces;
using SDTechnicalAssessment.Domain.Entities;
using SDTechnicalAssessment.Infrastructure.Data.Repositories;

namespace SDTechnicalAssessment.Application.Services
{
    // Service contains the business logic for Product operations.
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        // Repository is injected through Dependency Injection.
        public ProductService(
     IProductRepository repository,
     ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Returns all products.
        public async Task<PaginationDto<ProductDto>> GetAllAsync(
    int pageNumber,
    int pageSize)
        {

            // Log the pagination request.
            _logger.LogInformation(
                "Fetching products. PageNumber: {PageNumber}, PageSize: {PageSize}",
                pageNumber,
                pageSize);


            // Ask the repository for only the records
            // required for the requested page.
            var result = await _repository.GetAllAsync(
                pageNumber,
                pageSize);

            // Convert database entities into DTOs.
            var products = result.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    CreatedBy = p.CreatedBy,
                    CreatedOn = p.CreatedOn,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedOn = p.ModifiedOn
                })
                .ToList();

            // Calculate how many pages are available.
            var totalPages = (int)Math.Ceiling(
                result.TotalRecords / (double)pageSize);

            // Return the paginated response.
            return new PaginationDto<ProductDto>
            {
                Data = products,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = result.TotalRecords,
                TotalPages = totalPages
            };
        }

        // Returns one product.
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CreatedBy = product.CreatedBy,
                CreatedOn = product.CreatedOn,
                ModifiedBy = product.ModifiedBy,
                ModifiedOn = product.ModifiedOn
            };
        }

        // Creates a new product.
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                CreatedBy = dto.CreatedBy,

                // Server generates creation date.
                CreatedOn = DateTime.UtcNow
            };

            var createdProduct = await _repository.AddAsync(product);

            _logger.LogInformation(
                "Product created successfully. ProductId: {ProductId}, ProductName: {ProductName}",
                createdProduct.Id,
                createdProduct.ProductName);

            return new ProductDto
            {
                Id = createdProduct.Id,
                ProductName = createdProduct.ProductName,
                CreatedBy = createdProduct.CreatedBy,
                CreatedOn = createdProduct.CreatedOn
            };
        }

        // Updates an existing product.
        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            // First, find the product using the given ID.
            var product = await _repository.GetByIdAsync(id);

            // If product does not exist, log a warning and return false.
            if (product == null)
            {
                _logger.LogWarning(
                    "Product not found for update. ProductId: {ProductId}",
                    id);

                return false;
            }

            // Update the product details.
            product.ProductName = dto.ProductName;
            product.ModifiedBy = dto.ModifiedBy;
            product.ModifiedOn = DateTime.UtcNow;

            // Save the updated product to the database.
            await _repository.UpdateAsync(product);

            // Log successful update.
            _logger.LogInformation(
                "Product updated successfully. ProductId: {ProductId}",
                id);

            return true;
        }

        // Deletes a product.
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning(
                    "Product not found for deletion. ProductId: {ProductId}",
                    id);

                return false;
            }

            await _repository.DeleteAsync(product);

            _logger.LogInformation(
                "Product deleted successfully. ProductId: {ProductId}",
                id);

            return true;
        }
    }
}