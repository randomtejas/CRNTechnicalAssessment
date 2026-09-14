using Microsoft.EntityFrameworkCore;
using SDTechnicalAssessment.Domain.Entities;

namespace SDTechnicalAssessment.Infrastructure.Data.Repositories
{
    // This class contains the actual database operations for Product.
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        // DbContext is injected through Dependency Injection.
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Gets all products.
        // AsNoTracking improves performance because we are only reading data.
        public async Task<(List<Product> Products, int TotalRecords)> GetAllAsync(
     int pageNumber,
     int pageSize)
        {
            // Get the total number of products available
            // before applying pagination.
            var totalRecords = await _context.Products
                .CountAsync();

            // Calculate how many records should be skipped.
            var skip = (pageNumber - 1) * pageSize;

            // Fetch only the records required for the current page.
            var products = await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Return both the current page data
            // and the total record count.
            return (products, totalRecords);
        }

        // Gets a single product by Id.
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Adds a new product to the database.
        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        // Updates an existing product.
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // Deletes an existing product.
        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}