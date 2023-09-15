// discord parts
var discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

if (discordToken == null)
{
Console.WriteLine("DISCORD_TOKEN isn't set");
return;
}

var web = new Web();
var bot = new Bot(discordToken);
await bot.Start();