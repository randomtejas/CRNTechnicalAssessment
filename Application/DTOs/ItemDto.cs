namespace SDTechnicalAssessment.Application.DTOs
{
    // DTO used to return item information.
    public class ItemDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string ProductName { get; set; } = string.Empty;
    }
}