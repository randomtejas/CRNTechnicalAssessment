using SDTechnicalAssessment.Domain.Entities;

namespace SDTechnicalAssessment.Infrastructure.Data.Repositories
{
    // This interface defines all database operations required for Item.
    // The actual implementation will be created separately.
    public interface IItemRepository
    {
        // Gets all items from the database.
        Task<List<Item>> GetAllAsync();

        // Gets one item using its Id.
        Task<Item?> GetByIdAsync(int id);

        // Adds a new item.
        Task<Item> AddAsync(Item item);

        // Updates an existing item.
        Task UpdateAsync(Item item);

        // Deletes an item.
        Task DeleteAsync(Item item);
    }
}