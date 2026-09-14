namespace SDTechnicalAssessment.Domain.Entities
{
    // This class represents the Product table in our SQL Server database.
    // Entity Framework Core uses this class to map C# properties
    // to columns in the Product database table.
    public class Product
    {
        // Primary key of the Product table.
        // EF Core will automatically treat "Id" as the primary key.
        public int Id { get; set; }

        // Stores the name of the product.
        // This will be a required field in our database.
        public string ProductName { get; set; } = string.Empty;

        // Stores the user/person who created the product.
        public string CreatedBy { get; set; } = string.Empty;

        // Stores the date and time when the product was created.
        public DateTime CreatedOn { get; set; }

        // Stores the user/person who last modified the product.
        // Nullable because a newly created product may not have been modified yet.
        public string? ModifiedBy { get; set; }

        // Stores the date and time when the product was last modified.
        // Nullable because a newly created product may not have been modified yet.
        public DateTime? ModifiedOn { get; set; }


        // Navigation property representing the one-to-many relationship.
        // One Product can have multiple Items.
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
