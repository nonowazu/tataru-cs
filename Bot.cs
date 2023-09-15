using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;

public class Bot
{
    private DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private string _discordToken;

    public Bot(string discordToken)
    {
        _discordToken = discordToken;
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

    public async Task Start()
    {
        await _client.LoginAsync(TokenType.Bot, _discordToken);
        await _client.StartAsync();
    }

    private IServiceProvider ConfigureServices()
    {
        var map = new ServiceCollection();
            //.AddSingleton()

        return map.BuildServiceProvider();
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
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