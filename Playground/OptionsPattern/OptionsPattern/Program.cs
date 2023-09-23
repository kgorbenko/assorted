using Microsoft.Extensions.Options;

namespace OptionsPattern;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<Config>(builder.Configuration.GetSection("Config"));
        builder.Services.AddSingleton<SingletonService>();
        builder.Services.AddScoped<ScopedService>();

        var app = builder.Build();

        app.MapGet("/ioptions", (IOptions<Config> configOptions) => configOptions.Value);

        app.MapGet("/ioptionssnapshot-singleton", (SingletonService service) => service.GetSnapshot());
        app.MapGet("/optionsmonitor-singleton", (SingletonService service) => service.GetMonitor());

        app.MapGet("/ioptionssnapshot-scoped", (ScopedService service) => service.GetSnapshot());
        app.MapGet("/optionsmonitor-scoped", (ScopedService service) => service.GetMonitor());

        app.Run();
    }

    public class Config
    {
        public int IntProperty { get; set; }
        public string? StringProperty { get; set; }
    }

    public class SingletonService
    {
        private readonly IOptionsSnapshot<Config> optionsSnapshot;
        private readonly IOptionsMonitor<Config> optionsMonitor;

        public SingletonService(IOptionsSnapshot<Config> optionsSnapshot, IOptionsMonitor<Config> optionsMonitor)
        {
            this.optionsSnapshot = optionsSnapshot;
            this.optionsMonitor = optionsMonitor;
        }

        public Config GetSnapshot() => optionsSnapshot.Value;
        public Config GetMonitor() => optionsMonitor.CurrentValue;
    }

    public class ScopedService
    {
        private readonly IOptionsSnapshot<Config> optionsSnapshot;
        private readonly IOptionsMonitor<Config> optionsMonitor;

        public ScopedService(IOptionsSnapshot<Config> optionsSnapshot, IOptionsMonitor<Config> optionsMonitor)
        {
            this.optionsSnapshot = optionsSnapshot;
            this.optionsMonitor = optionsMonitor;
        }

        public Config GetSnapshot() => optionsSnapshot.Value;
        public Config GetMonitor() => optionsMonitor.CurrentValue;
    }
}