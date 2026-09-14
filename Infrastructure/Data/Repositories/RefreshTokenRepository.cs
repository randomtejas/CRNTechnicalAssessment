using Microsoft.EntityFrameworkCore;
using SDTechnicalAssessment.Domain.Entities;

namespace SDTechnicalAssessment.Infrastructure.Data.Repositories
{
    // This class handles database operations related to refresh tokens.
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        // ApplicationDbContext is injected through dependency injection.
        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Saves a new refresh token into the database.
        public async Task<RefreshToken> AddAsync(
            RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        // Finds a refresh token using its token value.
        public async Task<RefreshToken?> GetByTokenAsync(
            string token)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token);
        }

        // Updates an existing refresh token.
        // We will use this mainly when rotating/revoking tokens.
        public async Task UpdateAsync(
            RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);

            await _context.SaveChangesAsync();
        }
    }
}