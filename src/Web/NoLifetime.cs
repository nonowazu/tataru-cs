
using Microsoft.AspNetCore.Components;

namespace Tataru.Web;

/// <summary>
/// HACK to avoid running two lifetime management systems simultaneously. Can be removed when the web host and the generic host become one and the same again.
/// </summary>
public sealed class NoLifetime : IHostLifetime, IDisposable
{
    public void Dispose()
    {

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void OnCancelKeyPressed(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
    }
}
