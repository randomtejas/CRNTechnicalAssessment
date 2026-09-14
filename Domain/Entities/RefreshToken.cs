namespace SDTechnicalAssessment.Domain.Entities
{
    // This class represents a refresh token stored in the database.
    public class RefreshToken
    {
        // Primary key of the RefreshTokens table.
        public int Id { get; set; }

        // The user to whom this refresh token belongs.
        public int UserId { get; set; }

        // The actual refresh token value.
        public string Token { get; set; } = string.Empty;

        // Date and time until which the refresh token is valid.
        public DateTime ExpiresOn { get; set; }

        // Date and time when the refresh token was created.
        public DateTime CreatedOn { get; set; }

        // When this value is null, the token is still active.
        // When it has a value, the token has been revoked.
        public DateTime? RevokedOn { get; set; }

        // Navigation property.
        // This connects the refresh token to the User entity.
        public User User { get; set; } = null!;
    }
}