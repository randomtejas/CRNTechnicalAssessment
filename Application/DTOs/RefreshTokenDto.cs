namespace SDTechnicalAssessment.Application.DTOs
{
    // This DTO contains the refresh token sent by the client
    // when requesting a new access token.
    public class RefreshTokenDto
    {
        // The refresh token received during login.
        public string RefreshToken { get; set; } = string.Empty;
    }
}