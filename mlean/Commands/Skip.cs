using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using mlean.Audio;

namespace mlean.Commands
{
    public class Skip(IAudioService audioService, DiscordSocketClient discordClient)
        : CommandBase(audioService)
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
            
            AudioManager.Initialize(AudioService, Context, discordClient);

            if (player.Queue.Count > 0)
            {
                AudioManager.UpdateTrackEvent();
                await player.SkipAsync();
                await AudioManager.UpdateBotStatusAsync(player.CurrentTrack);
                await ReplyAsync(embed: Utilities.StatusEmbed("⏩ Skipped to the next track."));
            }
            else
            {
                await player.StopAsync();
                await AudioManager.UpdateBotStatusAsync();
                await ReplyAsync(embed: Utilities.StatusEmbed("🛑 No more tracks. Stopped playback."));
            }
        }
    }
}