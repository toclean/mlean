using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace mlean;

public class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<DiscordSocketClient>(provider =>
                {
                    var config = new DiscordSocketConfig
                    {
                        GatewayIntents = GatewayIntents.All
                    };
                    var client = new DiscordSocketClient(config);
                    client.Log += LogAsync;
                    return client;
                });

                // Configure Lavalink
                services.AddLavalink();
                services.ConfigureLavalink(options =>
                {
                    options.BaseAddress = new Uri("http://lavalink:2333");
                    options.WebSocketUri = new Uri("ws://lavalink:2333/v4/websocket");
                    options.Passphrase = "youshallnotpass";
                });

                services
                    .AddSingleton<AudioService>()
                    .AddSingleton<CommandService>()
                    .AddSingleton<CommandHandler>() // Add this line
                    .AddLogging(logging =>
                        logging.AddConsole().SetMinimumLevel(LogLevel.Trace));

            })
            .Build();

        var client = host.Services.GetRequiredService<DiscordSocketClient>();

        // Register commands
        var commandHandler = host.Services.GetRequiredService<CommandHandler>();
        await commandHandler.InitializeAsync();


        // Login and start the bot
        await client.LoginAsync(TokenType.Bot, Constants.Token);
        await client.StartAsync();

        // Run the host
        await host.RunAsync();
    }

    public static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }
}
