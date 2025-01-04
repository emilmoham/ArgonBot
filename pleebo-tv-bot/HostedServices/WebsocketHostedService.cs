using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.EventSub;

namespace pleebo_tv_bot.HostedServices
{
    public class WebsocketHostedService : IHostedService
    {
        
        private readonly ILogger<WebsocketHostedService> _logger;
        private readonly EventSubWebsocketClient _eventSubWebsocketClient;

        private readonly TwitchAPI _api;

        private readonly string _clientId = "";
        private readonly string _accessToken = "";
        private readonly string _chatChannelUserId = "";
        private readonly string _botUserId = "";


        public WebsocketHostedService(ILogger<WebsocketHostedService> logger, EventSubWebsocketClient eventSubWebsocketClient)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _api = new TwitchAPI();
            _api.Settings.ClientId = _clientId;
            _api.Settings.AccessToken = _accessToken;

            _eventSubWebsocketClient = eventSubWebsocketClient ?? throw new ArgumentNullException(nameof(eventSubWebsocketClient));
            _eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
            _eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
            _eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
            _eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

            _eventSubWebsocketClient.ChannelChatMessage += OnChatMessage;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _eventSubWebsocketClient.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventSubWebsocketClient.DisconnectAsync();
        }

        private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
        {
            _logger.LogInformation($"Websocket {_eventSubWebsocketClient.SessionId} connected!");

            if (!e.IsRequestedReconnect)
            {
                _logger.LogInformation("Requested reconnect");

                try { 
                    var x = await _api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                        "channel.chat.message",
                        "1",
                        new Dictionary<string, string>()
                        {
                            ["broadcaster_user_id"] = _chatChannelUserId,
                            ["user_id"] = _botUserId
                        },
                        EventSubTransportMethod.Websocket,
                        _eventSubWebsocketClient.SessionId);
                } catch (Exception ee)
                {
                    _logger.LogError(ee, "error subscribing");
                }

                _logger.LogInformation("Subscription Created");
            }

            
        }

        private async Task OnWebsocketDisconnected(object sender, EventArgs e)
        {
            _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} disconnected!");

            // Don't do this in production. You should implement a better reconnect strategy with exponential backoff
            while (!await _eventSubWebsocketClient.ReconnectAsync())
            {
                _logger.LogError("Websocket reconnect failed!");
                await Task.Delay(1000);
            }
        }

        private async Task OnWebsocketReconnected(object sender, EventArgs e)
        {
            _logger.LogWarning($"Websocket {_eventSubWebsocketClient.SessionId} reconnected");
        }

        private async Task OnErrorOccurred(object sender, ErrorOccuredArgs e)
        {
            _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} - Error occurred!");
        }

        private async Task OnChatMessage(object sender, ChannelChatMessageArgs e)
        {
            if (e.Notification.Payload.Event.Message.Text.Trim() == "HeyGuys")
            {
                await _api.Helix.Chat.SendChatMessage(
                    _chatChannelUserId,
                    _botUserId,
                    "VoHiYo");
            }
        }
    }
    
}
