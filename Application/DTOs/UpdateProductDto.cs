namespace SDTechnicalAssessment.Application.DTOs
{
    // DTO used when updating a product.
    public class UpdateProductDto
    {
        // New product name.
        public string ProductName { get; set; } = string.Empty;

        // Person modifying the product.
        public string ModifiedBy { get; set; } = string.Empty;
    }
}