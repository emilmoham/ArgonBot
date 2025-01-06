using ArgonBot.Models.Entities;
using Microsoft.EntityFrameworkCore;
using TwitchLib.EventSub.Websockets.Handler.Channel.Raids;

namespace ArgonBot.Repositories
{
    public interface IUserRepository
    {
        User AddUser(long userId, string userName);
        Task<IEnumerable<User>> GetUsersAsync(IEnumerable<long> ids);
        Task<User?> GetUserByIdAsync(long userId);
        Task<User?> GetUserByNameAsync(string userName);
        Task<User?> SetUserPointsAsync(long userId, uint points);
        Task<int> SaveChangesAsync();
    }

    public class UserRepository: IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly ApplicationDbContext _context;
        public UserRepository(ILogger<UserRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public User AddUser(long userId, string userName)
        {
            User user = new User()
            {
                UserId = userId,
                UserName = userName
            };

            _context.Add(user);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(IEnumerable<long> ids)
        {
            return await _context.Users.Where(e => ids.Contains(e.UserId)).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByNameAsync(string userName)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User?> SetUserPointsAsync(long userId, uint points)
        {
            User? user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("UserId: {0} not found", userId);
                return null;
            }

            user.ChannelPoints = points;
            return user;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
