namespace SDTechnicalAssessment.Application.DTOs
{
    public class LoginResponseDto
    {
        // Short-lived token used to access protected APIs.
        public string AccessToken { get; set; } = string.Empty;

        // Refresh token is used to obtain a new access token
        // after the access token expires.
        public string RefreshToken { get; set; } = string.Empty;

        // Access token validity period.
        public int ExpiresInMinutes { get; set; }

        // Role of the authenticated user.
        public string Role { get; set; } = string.Empty;
    }
}