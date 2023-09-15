using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;

public class Program
{
    private static DiscordSocketClient _client;
    private static CommandService _commands;
    private static IServiceProvider _services;

    public static async Task Main(string[] args)
    {
        // discord parts
        _client = new DiscordSocketClient();
        _client.Log += Log;
        var discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

        if (discordToken == null)
        {
            Console.WriteLine("DISCORD_TOKEN isn't set");
            return;
        }

        _commands = new CommandService(new CommandServiceConfig
        {
            // Log level
            LogLevel = LogSeverity.Debug,
            CaseSensitiveCommands = false,
        });
        _services = ConfigureServices();
        _commands.Log += Log;

        await _client.LoginAsync(TokenType.Bot, discordToken);
        await _client.StartAsync();

        // asp.net parts
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }

    private static IServiceProvider ConfigureServices()
    {
        var map = new ServiceCollection();
            //.AddSingleton()

        return map.BuildServiceProvider();
    }

    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private static async Task InitCommands()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        _client.MessageReceived += HandleCommandAsync;
    }

    private static async Task HandleCommandAsync(SocketMessage arg)
    {
        var msg = arg as SocketUserMessage;
        if (msg == null) return;

        // don't respond to ourselves or another bot, though the latter would be funny
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

        var context = new SocketCommandContext(_client, msg);
        var result = await _commands.ExecuteAsync(context, 0, _services);
    }
}