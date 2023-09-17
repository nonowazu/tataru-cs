namespace Tataru;

public class Web : IHostedService
{
    private WebApplication _app;

    public Web()
    {
        // TODO: pass args here
        var builder = WebApplication.CreateBuilder();
        // Add services to the container.
        builder.Services
            .AddSingleton<IHostLifetime, NoLifetime>()
            .AddRazorPages();

        _app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!_app.Environment.IsDevelopment())
        {
            _app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _app.UseHsts();
        }

        _app.UseHttpsRedirection();
        _app.UseStaticFiles();

        _app.UseRouting();

        _app.UseAuthorization();

        _app.MapRazorPages();
    }

    public async Task RunAsync()
    {
        await _app.RunAsync();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _app.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _app.StopAsync(cancellationToken);
    }
}