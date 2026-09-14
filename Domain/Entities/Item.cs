namespace SDTechnicalAssessment.Domain.Entities
{
    // This class represents the Item table in the database.
    // An Item belongs to a Product and stores its quantity.
    public class Item
    {
        // Primary key of the Item table.
        public int Id { get; set; }

        // Foreign key that identifies the Product associated with this Item.
        public int ProductId { get; set; }

        // Stores the quantity of the product.
        public int Quantity { get; set; }

        // Navigation property.
        // This allows Entity Framework Core to navigate from Item to Product.
        public Product Product { get; set; } = null!;
    }
}