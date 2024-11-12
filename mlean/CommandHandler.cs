using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace mlean;

public class CommandHandler
{
    private readonly CommandService _commandService;
    private readonly DiscordSocketClient _discordClient;
    private readonly IServiceProvider _services;

    // Retrieve prefix from environment variable or default to "!"
    private static readonly string Prefix = Environment.GetEnvironmentVariable("BOT_PREFIX") ?? "!";

    public CommandHandler(DiscordSocketClient discordClient, CommandService commandService, IServiceProvider services)
    {
        _commandService = commandService;
        _discordClient = discordClient;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        _discordClient.MessageReceived += HandleCommandAsync;
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    public void PrintCommands()
    {
        foreach (var commandServiceCommand in _commandService.Commands.DistinctBy(x => x.Name))
        {
            Console.WriteLine(commandServiceCommand.Name + " : " + commandServiceCommand.Summary);
        }
    }

    private async Task HandleCommandAsync(SocketMessage socketMessage)
    {
        // Ensure message is a user message and not from a bot
        if (socketMessage is not SocketUserMessage message || message.Author.IsBot) return;

        var argPos = 0;

        // Check for prefix or mention-based commands
        if (!(message.HasStringPrefix(Prefix, ref argPos) || 
              message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos))) return;

        var context = new SocketCommandContext(_discordClient, message);

        // Execute the command and handle errors
        var result = await _commandService.ExecuteAsync(context, argPos, _services);
        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
        {
            await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}