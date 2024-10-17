using Discord;
using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Protocol.Payloads.Events;
using mlean.Audio;

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
                await AudioManager.UpdateBotStatusAsync(player.CurrentTrack);
                await ReplyAsync(embed: Utilities.StatusEmbed("⏩ Skipped to the next track."));
                AudioManager.UpdateTrackEvent();
            }
            else
            {
                await player.StopAsync();
                await AudioManager.UpdateBotStatusAsync();
                await ReplyAsync(embed: Utilities.StatusEmbed("🛑 No more tracks. Stopped playback."));
            }
        }

        private async Task AudioServiceOnTrackEnded(object sender, TrackEndedEventArgs eventargs)
        {
            Console.WriteLine($"TrackEndReason: {eventargs.Reason} MayStartNext: {eventargs.MayStartNext}");
            if (eventargs.Reason == TrackEndReason.Finished)
            {
                if (eventargs.Player.CurrentTrack != null)
                    await ReplyAsync(embed: Utilities.CreateTrackEmbed(eventargs.Player.CurrentTrack, "Now Playing"));
            }
        }
    }
}