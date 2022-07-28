using Backlang.Driver;
using LSP_Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using System.Diagnostics;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Debugger.Break();

        var server = await LanguageServer.From(options =>
            options
                .WithInput(Console.OpenStandardInput())
                .WithOutput(Console.OpenStandardOutput())
                .WithLoggerFactory(new LoggerFactory())
                .AddDefaultLoggingProvider()
                .WithServices(ConfigureServices)
                .WithHandler<TextDocumentSyncHandler>()
                .WithHandler<CompletionHandler>()
                .WithHandler<HoverHandler>()
                .WithServerInfo(new ServerInfo() { Name = "Backlang LSP", Version = "1.0.0.0" })
             );

        await server.WasShutDown;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<BufferManager>();
        services.AddSingleton<TextDocumentSyncHandler>();

        services.AddSingleton(PluginContainer.Load());
    }
}