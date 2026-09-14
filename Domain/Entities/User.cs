namespace SDTechnicalAssessment.Domain.Entities
{
    // ------------------------------------------------------------
    // User entity represents a user who can log in to our API.
    // ------------------------------------------------------------
    public class User
    {
        // Primary key of the User table.
        public int Id { get; set; }

        // Username used during login.
        public string Username { get; set; } = string.Empty;

        // Password stored for this assessment.
        //
        // IMPORTANT:
        // In a real production application, passwords should
        // never be stored as plain text.
       // Password is stored as a secure hash using ASP.NET Core PasswordHasher.
        public string Password { get; set; } = string.Empty;

        // Role of the user.
        // Example: Admin or User.
        public string Role { get; set; } = "User";

        public ICollection<RefreshToken> RefreshTokens { get; set; }
    = new List<RefreshToken>();
    }
}