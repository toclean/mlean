using Discord;
using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;

namespace mlean.Commands
{
    public class Skip(IAudioService audioService, DiscordSocketClient discordClient)
        : CommandBase(audioService, discordClient)
    {
        [Command("skip", RunMode = RunMode.Async)]
        public async Task SkipAsync()
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }

            if (player.Queue.Count > 0)
            {
                await player.SkipAsync();
                await UpdateBotStatusAsync(player.CurrentTrack);
                await ReplyAsync(embed: Utilities.StatusEmbed("⏩ Skipped to the next track."));
            }
            else
            {
                await player.StopAsync();
                await UpdateBotStatusAsync();
                await ReplyAsync(embed: Utilities.StatusEmbed("🛑 No more tracks. Stopped playback."));
            }
        }
    }
}