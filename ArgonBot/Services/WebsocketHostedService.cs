﻿using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace ArgonBot.Services
{
    public class WebsocketHostedService : IHostedService
    {

        private readonly ILogger<WebsocketHostedService> _logger;
        private readonly EventSubWebsocketClient _eventSubWebsocketClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly TwitchApiService _twitchApiService;

        public WebsocketHostedService(
            ILogger<WebsocketHostedService> logger,
            EventSubWebsocketClient eventSubWebsocketClient,
            TwitchApiService twitchApiService,
            IServiceProvider serviceProvider
        )
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider;
            _twitchApiService = twitchApiService;

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
                await _twitchApiService.SubscribeToChatMessages(_eventSubWebsocketClient.SessionId);
                _logger.LogInformation("Subscribed to chat messages");
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
                await _twitchApiService.SendChatMessage("VoHiYo");
                return;
            }

            await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            UserService userService = scope.ServiceProvider.GetRequiredService<UserService>();
            if (e.Notification.Payload.Event.Message.Text.Trim() == "!points")
            {
                long userId = long.Parse(e.Notification.Payload.Event.ChatterUserId);
                string userName = e.Notification.Payload.Event.ChatterUserName;
                uint userPoints = await userService.GetUsersChannelPoints(userId);
                await _twitchApiService.SendChatMessage(string.Format("{0} has {1} points", userName, userPoints));
            }
        }
    }

}
