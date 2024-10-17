using Discord;
using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Protocol.Payloads.Events;
using mlean.Audio;

namespace mlean.Commands
{
    public class Skip(IAudioService audioService)
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