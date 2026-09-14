using SDTechnicalAssessment.Application.DTOs;

namespace SDTechnicalAssessment.Application.Interfaces
{
    // Defines business operations for Product.
    public interface IProductService
    {
        Task<PaginationDto<ProductDto>> GetAllAsync(
    int pageNumber,
    int pageSize);

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto> CreateAsync(CreateProductDto dto);

        Task<bool> UpdateAsync(int id, UpdateProductDto dto);

        Task<bool> DeleteAsync(int id);
    }
}