namespace Tataru.Bot;

/// <summary>
/// Implementation of the <see cref="ITokenProvider"/> interface to provide a Discord Bot Token.
/// </summary>
/// <param name="discordToken">The Discord Bot Token to provide. Do not hardcode tokens, as that is a security issue.</param>
public class DiscordTokenProvider(string discordToken) : ITokenProvider
{
    public string Token { get; init; } = discordToken;
}
