namespace SDTechnicalAssessment.Application.DTOs
{
    // ------------------------------------------------------------
    // This DTO represents the data required when a user
    // tries to log in to our API.
    // ------------------------------------------------------------
    public class LoginDto
    {
        // Username entered by the user.
        public string Username { get; set; } = string.Empty;

        // Password entered by the user.
        public string Password { get; set; } = string.Empty;
    }
}