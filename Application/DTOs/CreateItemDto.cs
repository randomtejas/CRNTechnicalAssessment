namespace SDTechnicalAssessment.Application.DTOs
{
    // DTO used when creating a new item.
    public class CreateItemDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}