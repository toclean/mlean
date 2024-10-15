using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using ContextType = Discord.Interactions.ContextType;

namespace mlean.Commands;

[Discord.Interactions.RequireContext(ContextType.Guild)]
public class AudioModule : ModuleBase<SocketCommandContext>
{
    private readonly IAudioService _audioService;
    private readonly DiscordSocketClient _discordClient;

    public AudioModule(IAudioService audioService, DiscordSocketClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(bool join = false)
    {
        var options = new PlayerRetrieveOptions(
            ChannelBehavior: join ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var result = await _audioService.Players.RetrieveAsync(Context, PlayerFactory.Queued, options);
        if (result.IsSuccess) return result.Player;

        await ReplyAsync(embed: Utilities.ErrorEmbed(result.Status switch
        {
            PlayerRetrieveStatus.UserNotInVoiceChannel => "You're not in a voice channel.",
            PlayerRetrieveStatus.BotNotConnected => "The bot isn't connected.",
            _ => "An unknown error occurred."
        }));

        return null;
    }
}