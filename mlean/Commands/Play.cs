using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mlean.Commands
{
    public class Play : CommandBase
    {
        private readonly DiscordSocketClient _discordClient;

        public Play(IAudioService audioService, DiscordSocketClient discordClient) : base(audioService)
        {
            _discordClient = discordClient;
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Remainder] string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Provide a search query."));
                return;
            }

            // Check if the search query is a YouTube playlist URL
            bool isPlaylist = Regex.IsMatch(searchQuery, @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com|youtu\.be)\/.*(?:list=)([a-zA-Z0-9_-]+)");

            var trackSearchMode = isPlaylist ? TrackSearchMode.YouTube : TrackSearchMode.YouTube; // Use YouTube search mode in both cases
            var tracks = await AudioService.Tracks.LoadTracksAsync(searchQuery, trackSearchMode);

            if (!tracks.HasMatches)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed($"No results for `{searchQuery}`."));
                return;
            }

            var player = await GetPlayerAsync(true);
            AudioManager.Initialize(AudioService, Context, _discordClient);

            if (player == null) return;

            // If it's a YouTube playlist URL, handle it as a playlist
            if (isPlaylist && tracks.Tracks.Length > 1)
            {
                var firstTrack = tracks.Tracks.First(x => !x.IsLiveStream && x.IsSeekable);

                if (player.State != PlayerState.Playing)
                {
                    await player.PlayAsync(firstTrack);

                    if (!AudioManager.Volume)
                    {
                        await player.SetVolumeAsync(0.1f);
                        AudioManager.Volume = true;
                    }

                    AudioManager.UpdateTrackEvent();

                    // Enqueue the rest of the tracks
                    foreach (var track in tracks.Tracks.Skip(1).Where(x => !x.IsLiveStream && x.IsSeekable))
                    {
                        await player.Queue.AddAsync(new TrackQueueItem(track));
                    }

                    await ReplyAsync(embed: Utilities.CreatePlaylistEmbed(tracks.Tracks.Length, "Started playing playlist and added to queue"));
                }
                else
                {
                    // If player is already playing, add all tracks to the queue
                    foreach (var track in tracks.Tracks.Where(x => !x.IsLiveStream && x.IsSeekable))
                    {
                        await player.Queue.AddAsync(new TrackQueueItem(track));
                    }

                    await ReplyAsync(embed: Utilities.CreatePlaylistEmbed(tracks.Tracks.Length, "Playlist added to queue"));
                }
            }
            else
            {
                // Single track case
                var track = tracks.Tracks.First(x => !x.IsLiveStream && x.IsSeekable);

                if (player.State == PlayerState.Playing)
                {
                    await player.Queue.AddAsync(new TrackQueueItem(track));
                    await ReplyAsync(embed: Utilities.CreateTrackEmbed(track, "Added to Queue"));
                }
                else
                {
                    await player.PlayAsync(track);

                    if (!AudioManager.Volume)
                    {
                        await player.SetVolumeAsync(0.1f);
                        AudioManager.Volume = true;
                    }

                    AudioManager.UpdateTrackEvent();
                }
            }
        }
    }
}
