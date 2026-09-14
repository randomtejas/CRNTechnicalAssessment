using SDTechnicalAssessment.Domain.Entities;

namespace SDTechnicalAssessment.Infrastructure.Data.Repositories
{
    // This interface defines all database operations required for Product.
    // The actual implementation will be created separately.
    public interface IProductRepository
    {
        // Gets all products from the database.
        Task<(List<Product> Products, int TotalRecords)> GetAllAsync(
     int pageNumber,
     int pageSize);

        // Gets one product using its Id.
        Task<Product?> GetByIdAsync(int id);

        // Adds a new product.
        Task<Product> AddAsync(Product product);

        // Updates an existing product.
        Task UpdateAsync(Product product);

        // Deletes a product.
        Task DeleteAsync(Product product);
    }
}