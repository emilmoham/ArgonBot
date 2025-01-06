
using ArgonBot.Models.Entities;
using TwitchLib.Api.Helix.Models.Chat.GetChatters;

namespace ArgonBot.Services
{
    public class ChannelPointDistributionBackgroundService : BackgroundService
    {
        private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        private readonly uint pointsPerTick = 10;

        private ILogger<ChannelPointDistributionBackgroundService> _logger;
        private IServiceProvider _serviceProvider;
        private TwitchApiService _twitchApiService;
        //private UserService _userService;

        public ChannelPointDistributionBackgroundService(
            ILogger<ChannelPointDistributionBackgroundService> logger,
            IServiceProvider serviceProvider,
            TwitchApiService twitchApiService
            //UserService userService
        ) {
            _logger = logger;
            _twitchApiService = twitchApiService;
            _serviceProvider = serviceProvider;
            //_userService = userService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            UserService userService = scope.ServiceProvider.GetRequiredService<UserService>();
            while (await _timer.WaitForNextTickAsync(stoppingToken) 
                    && !stoppingToken.IsCancellationRequested)
            {
                await DistributeChannelPoints(userService);
            }
        }

        private async Task DistributeChannelPoints(UserService userService)
        {
            _logger.LogInformation("Distribuitng channel points");
            GetChattersResponse chatters = await _twitchApiService.GetChatters();
            IEnumerable<User> users = await userService.GetUsersFromChatters(chatters.Data);
            await userService.DistributeChannelPoints(users, pointsPerTick);
        }
    }
}
