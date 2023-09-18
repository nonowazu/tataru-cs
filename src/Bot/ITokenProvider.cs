namespace Tataru.Bot;

/// <summary>
/// Interface to provide a token to dependency injection.
/// </summary>
public interface ITokenProvider
{
    string Token { get; init; }
}
