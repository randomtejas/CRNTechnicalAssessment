using SDTechnicalAssessment.Application.DTOs;

namespace SDTechnicalAssessment.Application.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<ItemDto>> GetAllAsync();

        Task<ItemDto?> GetByIdAsync(int id);

        Task<ItemDto> CreateAsync(CreateItemDto dto);

        Task<bool> UpdateAsync(int id, UpdateItemDto dto);

        Task<bool> DeleteAsync(int id);
    }
}