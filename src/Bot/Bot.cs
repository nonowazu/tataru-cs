using Discord; 
using Discord.WebSocket; 
using Discord.Commands; 
using System.Reflection; 

namespace Tataru.Bot; 

public class Bot : IHostedService 
{
    private DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly string _discordToken;
    private readonly ILogger<Bot> _logger;

    public Bot(ITokenProvider tokenProvider, ILogger<Bot> logger)
    {
        _discordToken = tokenProvider.Token;
        _logger = logger;                    
        _client = new DiscordSocketClient();
        _client.Log += Log;

        _commands = new CommandService(new CommandServiceConfig
        {
            // Log level
            LogLevel = LogSeverity.Debug,
            CaseSensitiveCommands = false,
        });
        _services = ConfigureServices();
        _commands.Log += Log;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.LoginAsync(TokenType.Bot, _discordToken);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // I do not like this, but if we're actually trying to gracefully shut down the bot, 
        // then the delay is necessary, because DiscordSocketClient.StopAsync() returns too 
        // quickly for the client to actually disconnect.
        await Task.WhenAll(_client.StopAsync(), Task.Delay(1000, cancellationToken));
    }

    private IServiceProvider ConfigureServices()
    {
        var map = new ServiceCollection();
            //.AddSingleton()

        return map.BuildServiceProvider();
    }

    private Task Log(LogMessage msg)
    {
        return msg.Severity switch
        {
            LogSeverity.Critical => Task.Run(() => _logger.LogCritical("{Message}", msg)),
            LogSeverity.Error => Task.Run(() => _logger.LogError("{Message}", msg)),
            LogSeverity.Warning => Task.Run(() => _logger.LogWarning("{Message}", msg)),
            LogSeverity.Debug => Task.Run(() => _logger.LogDebug("{Message}", msg)),
            LogSeverity.Info => Task.Run(() => _logger.LogInformation("{Message}", msg)),
            _ => Task.Run(() => _logger.LogTrace("{Message}", msg)),
        };
    }

    private async Task InitCommands()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        _client.MessageReceived += HandleCommandAsync;
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        if (arg is not SocketUserMessage msg) return;

        // don't respond to ourselves or another bot, though the latter would be funny
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

        var context = new SocketCommandContext(_client, msg);
        var result = await _commands.ExecuteAsync(context, 0, _services);
    }
}