// discord parts
using Tataru.Bot;
using Tataru.Web;

var discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

if (discordToken == null)
{
    Console.WriteLine("DISCORD_TOKEN isn't set");
    return;
}

// Create an application builder that runs DI, Lifetime control and configuration loading for us.
var builder = Host.CreateApplicationBuilder(args);

// Add services necessary for the different things that we want to run.
// IHostedServices like our bot and web service get automatically started and stopped when we call Start/Stop/Run on the host.
// Currently, we just run both, but we may want to decide not to run one or the other.
////if (runDiscordBot)
// Provide the token in a DI-able class and add the bot service which will use the token provider.
// FIXME: Maybe we should provide it via a configuration service that is automagically read and provided instead of like this?
builder.Services
    .AddSingleton<ITokenProvider>(_ => new DiscordTokenProvider(discordToken))
    .AddHostedService<Bot>();

////if (runWebServer)
// Add the web server service, which we ideally wouldn't start like this, because this is 
// spinning up a separate host, with all the trimmings the host in this file already provides.
builder.Services.AddHostedService<Web>();

using var host = builder.Build();

await host.RunAsync();