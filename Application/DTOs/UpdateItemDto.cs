namespace SDTechnicalAssessment.Application.DTOs
{
    // DTO used when updating an existing item.
    public class UpdateItemDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}