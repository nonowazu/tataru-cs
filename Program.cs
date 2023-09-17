// discord parts
var discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

if (discordToken == null)
{
    Console.WriteLine("DISCORD_TOKEN isn't set");
    return;
}

// Cancellation token
var source = new CancellationTokenSource();
var token = source.Token;
var tasks = new List<Task>();

Console.CancelKeyPress += new ConsoleCancelEventHandler((object? sender, ConsoleCancelEventArgs args) => {
    source.Cancel();
});

var web = new Web();
var bot = new Bot(discordToken);
tasks.Add(web.RunAsync());
tasks.Add(bot.StartAsync());

try
{
    Task.WaitAll([..tasks], token);
}
catch (OperationCanceledException) {} // Excepted on ^C/sigint
