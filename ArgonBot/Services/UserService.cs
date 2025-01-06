using ArgonBot.Models.Entities;
using ArgonBot.Repositories;
using TwitchLib.Api.Helix.Models.Chat.GetChatters;

namespace ArgonBot.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;

        public UserService(ILogger<UserService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetUsersFromChatters(IEnumerable<Chatter> chatters)
        {
            IEnumerable<long> chatterIds = chatters.Select(e => long.Parse(e.UserId));

            // Get the list of known user id's
            IEnumerable<User> knownUsers = await _userRepository.GetUsersAsync(chatterIds);
            IEnumerable<long> knownChatterIds = knownUsers.Select(e => e.UserId);

            // Add the new chatters to our database
            IEnumerable<Chatter> newChatters = chatters.Where(e => !knownChatterIds.Contains(long.Parse(e.UserId)));
            List<User> addedUsers = new();
            foreach (Chatter newChatter in newChatters)
            {
                addedUsers.Add(_userRepository.AddUser(long.Parse(newChatter.UserId), newChatter.UserName));
            }
            await _userRepository.SaveChangesAsync();

            knownUsers.Concat(addedUsers);
            return knownUsers;
        }

        public async Task<IEnumerable<User>> DistributeChannelPoints(IEnumerable<User> users, uint pointsToAdd)
        {
            foreach (User user in users)
            {
                user.ChannelPoints += pointsToAdd;
            }

            await _userRepository.SaveChangesAsync();
            return users;
        }

        public async Task<uint> GetUsersChannelPoints(long userId)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return 0;
            return user.ChannelPoints;
        }
    }
}
