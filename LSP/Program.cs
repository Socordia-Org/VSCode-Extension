using System.Diagnostics;
using Backlang.Contracts;
using LSP_Server.Core;
using LSP_Server.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;

namespace LSP_Server;

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
                .WithHandler<RenameHandler>()
                .WithHandler<SignatureHelpHandler>()
                .WithServerInfo(new ServerInfo { Name = "Backlang LSP", Version = "1.0.0.0" })
        );

        await server.WasShutDown;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<BufferManager>();
        services.AddSingleton<TextDocumentSyncHandler>();
        services.AddSingleton<Workspace>();

        services.AddSingleton(PluginContainer.Load());
    }
}