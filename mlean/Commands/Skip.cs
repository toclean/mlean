using Discord;
using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Protocol.Payloads.Events;

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
                audioService.TrackEnded += AudioServiceOnTrackEnded;
            }
            else
            {
                await player.StopAsync();
                await UpdateBotStatusAsync();
                await ReplyAsync(embed: Utilities.StatusEmbed("🛑 No more tracks. Stopped playback."));
            }
        }

        private async Task AudioServiceOnTrackEnded(object sender, TrackEndedEventArgs eventargs)
        {
            Console.WriteLine($"TrackEndReason: {eventargs.Reason} MayStartNext: {eventargs.MayStartNext}");
            if (eventargs.Reason == TrackEndReason.Finished)
            {
                await ReplyAsync(embed: Utilities.CreateTrackEmbed(eventargs.Player.CurrentTrack, "Now Playing"));
            }
        }
    }
}