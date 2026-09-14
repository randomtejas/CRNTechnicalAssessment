using Microsoft.EntityFrameworkCore;
using SDTechnicalAssessment.Domain.Entities;

namespace SDTechnicalAssessment.Infrastructure.Data.Repositories
{
    // This class contains the actual database operations for Item.
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;

        // DbContext is injected through Dependency Injection.
        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Gets all items.
        // Product is included because ItemDto needs ProductName.
        public async Task<List<Item>> GetAllAsync()
        {
            return await _context.Items
                .AsNoTracking()
                .Include(i => i.Product)
                .OrderBy(i => i.Id)
                .ToListAsync();
        }

        // Gets a single item by Id.
        // Product is included so that ProductName is available.
        public async Task<Item?> GetByIdAsync(int id)
        {
            return await _context.Items
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // Adds a new item to the database.
        public async Task<Item> AddAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            return item;
        }

        // Updates an existing item.
        public async Task UpdateAsync(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }

        // Deletes an existing item.
        public async Task DeleteAsync(Item item)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}