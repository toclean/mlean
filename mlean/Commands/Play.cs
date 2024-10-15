using Discord;
using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;

namespace mlean.Commands
{
    public class Play(IAudioService audioService, DiscordSocketClient discordClient)
        : CommandBase(audioService, discordClient)
    {
        

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Remainder] string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Provide a search query."));
                return;
            }

            var tracks = await _audioService.Tracks.LoadTracksAsync(searchQuery, TrackSearchMode.YouTube);
            if (!tracks.HasMatches)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed($"No results for `{searchQuery}`."));
                return;
            }

            var track = tracks.Tracks.First(x => !x.IsLiveStream && x.IsSeekable);
            var player = await GetPlayerAsync(true);
            if (player == null) return;

            if (player.State == PlayerState.Playing)
            {
                await player.Queue.AddAsync(new TrackQueueItem(track));
                await ReplyAsync(embed: Utilities.CreateTrackEmbed(track, "Added to Queue"));
            }
            else
            {
                await player.PlayAsync(track);
                await UpdateBotStatusAsync(track);
                await ReplyAsync(embed: Utilities.CreateTrackEmbed(track, "Now Playing"));
            }
        }
    }
}