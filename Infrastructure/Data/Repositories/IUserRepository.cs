using SDTechnicalAssessment.Domain.Entities;

namespace SDTechnicalAssessment.Infrastructure.Data.Repositories
{
    // ------------------------------------------------------------
    // Defines database operations related to Users.
    // ------------------------------------------------------------
    public interface IUserRepository
    {
        // --------------------------------------------------------
        // Finds a user using the username.
        //
        // Returns null when the username does not exist.
        // --------------------------------------------------------
        Task<User?> GetByUsernameAsync(string username);
    }
}