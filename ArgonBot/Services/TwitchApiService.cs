using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;

namespace ArgonBot.Services
{
    public class TwitchApiService
    {
        private readonly ILogger<TwitchApiService> _logger;
        private readonly TwitchAPI _api;

        private readonly string _clientId;
        private readonly string _accessToken;
        private readonly string _broadcasterId;
        private readonly string _botUserId;

        public TwitchApiService(ILogger<TwitchApiService> logger, IConfiguration configuration)
        {
            _logger = logger;

            string? clientId = configuration["Twitch:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
                throw new Exception("Twitch:ClientId does not have a valid value");
            _clientId = clientId;

            string? accessToken = configuration["Twitch:AccessToken"];
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new Exception("Twitch:Secret does not have a valid value");
            _accessToken = accessToken;

            string? broadcasterId = configuration["Twitch:BroadcasterId"];
            if (string.IsNullOrWhiteSpace(broadcasterId))
                throw new Exception("Twitch:BroadcasterId does not have a valid value");
            _broadcasterId = broadcasterId;

            string? botUserId = configuration["Twitch:BotUserId"];
            if (string.IsNullOrWhiteSpace(botUserId))
                throw new Exception("Twitch:BotUserId does not have a valid value");
            _botUserId = botUserId;


            _api = new TwitchAPI();
            _api.Settings.ClientId = clientId;
            _api.Settings.AccessToken = _accessToken;
        }

        public async Task SubscribeToChatMessages(string sessionId)
        {
            try
            {
                await _api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.chat.message",
                    "1",
                    new Dictionary<string, string>()
                    {
                        ["broadcaster_user_id"] = _broadcasterId,
                        ["user_id"] = _botUserId
                    },
                    EventSubTransportMethod.Websocket,
                    sessionId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error subscribing to chat messages");
            }
        }


        public async Task SendChatMessage(string chatMessage)
        {
            await _api.Helix.Chat.SendChatMessage(
                _broadcasterId,
                _botUserId,
                "VoHiYo");
        }
    }
}
