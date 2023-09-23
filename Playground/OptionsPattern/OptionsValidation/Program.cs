using Microsoft.Extensions.Options;

namespace OptionsValidation;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<Model1>(builder.Configuration.GetSection("Config1"));
        builder.Services
               .AddOptions<Model2>()
               .BindConfiguration("Config2")
               .ValidateOnStart();

        var app = builder.Build();

        app.MapGet("/runtime-validation-error", (IOptions<Model1> modelOptions) => modelOptions.Value);
        app.MapGet("/startup-validation-error", (IOptions<Model2> modelOptions) => modelOptions.Value);

        app.Run();
    }

    public class Model1
    {
        public int IntProperty { get; set; }
    }

    public class Model2
    {
        public int IntProperty { get; set; }
    }
}