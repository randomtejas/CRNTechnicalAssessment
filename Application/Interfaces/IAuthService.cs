using SDTechnicalAssessment.Application.DTOs;

namespace SDTechnicalAssessment.Application.Interfaces
{
    public interface IAuthService
    {
        // Authenticates the user and generates
        // an access token and refresh token.
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);

        // Validates the refresh token and generates
        // a new access token and refresh token.
        Task<LoginResponseDto?> RefreshTokenAsync(
            RefreshTokenDto dto);
    }
}