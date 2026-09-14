using Microsoft.EntityFrameworkCore;
using SDTechnicalAssessment.Domain.Entities;

namespace SDTechnicalAssessment.Infrastructure.Data.Repositories
{
    // ------------------------------------------------------------
    // Implements database operations related to Users.
    // ------------------------------------------------------------
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        // --------------------------------------------------------
        // ApplicationDbContext is injected through Dependency
        // Injection so this repository can access SQL Server.
        // --------------------------------------------------------
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // --------------------------------------------------------
        // Finds a user by username.
        //
        // AsNoTracking is used because we are only reading
        // the user and don't need Entity Framework to track it.
        // --------------------------------------------------------
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}