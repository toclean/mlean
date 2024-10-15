using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;

namespace mlean.Commands
{
    public class Leave(IAudioService audioService, DiscordSocketClient discordClient)
        : CommandBase(audioService, discordClient)
    {
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveAsync()
        {
            var player = await GetPlayerAsync();
            if (player != null) await player.DisconnectAsync();

            await UpdateBotStatusAsync();
            await ReplyAsync(embed: Utilities.StatusEmbed("Left the voice channel."));
        }
    }
}