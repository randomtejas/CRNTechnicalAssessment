using SDTechnicalAssessment.Application.DTOs;
using SDTechnicalAssessment.Application.Interfaces;
using SDTechnicalAssessment.Domain.Entities;
using SDTechnicalAssessment.Infrastructure.Data.Repositories;

namespace SDTechnicalAssessment.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;
        private readonly ILogger<ItemService> _logger;

        public ItemService(
            IItemRepository repository,
            ILogger<ItemService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ItemDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all items.");

            var items = await _repository.GetAllAsync();

            return items.Select(i => new ItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                ProductName = i.Product?.ProductName ?? string.Empty
            }).ToList();
        }

        public async Task<ItemDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
                return null;

            return new ItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductName = item.Product?.ProductName ?? string.Empty
            };
        }

        public async Task<ItemDto> CreateAsync(CreateItemDto dto)
        {
            var item = new Item
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            var createdItem = await _repository.AddAsync(item);

            _logger.LogInformation(
                "Item created successfully. ItemId: {ItemId}, ProductId: {ProductId}",
                createdItem.Id,
                createdItem.ProductId);

            // Reload item so ProductName is available.
            var itemWithProduct = await _repository.GetByIdAsync(createdItem.Id);

            return new ItemDto
            {
                Id = createdItem.Id,
                ProductId = createdItem.ProductId,
                Quantity = createdItem.Quantity,
                ProductName = itemWithProduct?.Product?.ProductName ?? string.Empty
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateItemDto dto)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
            {
                _logger.LogWarning(
                    "Item not found for update. ItemId: {ItemId}",
                    id);

                return false;
            }

            item.ProductId = dto.ProductId;
            item.Quantity = dto.Quantity;

            await _repository.UpdateAsync(item);

            _logger.LogInformation(
                "Item updated successfully. ItemId: {ItemId}",
                id);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
            {
                _logger.LogWarning(
                    "Item not found for deletion. ItemId: {ItemId}",
                    id);

                return false;
            }

            await _repository.DeleteAsync(item);

            _logger.LogInformation(
                "Item deleted successfully. ItemId: {ItemId}",
                id);

            return true;
        }
    }
}