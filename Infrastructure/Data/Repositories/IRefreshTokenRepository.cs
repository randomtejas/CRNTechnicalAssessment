using SDTechnicalAssessment.Domain.Entities;

namespace SDTechnicalAssessment.Infrastructure.Data.Repositories
{
    // This interface defines database operations for refresh tokens.
    public interface IRefreshTokenRepository
    {
        // Saves a new refresh token.
        Task<RefreshToken> AddAsync(RefreshToken refreshToken);

        // Finds a refresh token by its token value.
        Task<RefreshToken?> GetByTokenAsync(string token);

        // Updates an existing refresh token.
        Task UpdateAsync(RefreshToken refreshToken);
    }
}