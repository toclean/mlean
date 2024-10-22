using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using System.Linq;

namespace mlean.Commands
{
    public class Play(IAudioService audioService, DiscordSocketClient discordClient)
        : CommandBase(audioService)
    {

        [Command("play", RunMode = RunMode.Async)]
public async Task PlayAsync([Remainder] string searchQuery)
{
    if (string.IsNullOrWhiteSpace(searchQuery))
    {
        await ReplyAsync(embed: Utilities.ErrorEmbed("Provide a search query."));
        return;
    }

    var tracks = await AudioService.Tracks.LoadTracksAsync(searchQuery, TrackSearchMode.YouTube);
    if (!tracks.HasMatches)
    {
        await ReplyAsync(embed: Utilities.ErrorEmbed($"No results for `{searchQuery}`."));
        return;
    }

    var player = await GetPlayerAsync(true);
    AudioManager.Initialize(AudioService, Context, discordClient);

    if (player == null) return;

    // Check if it's a playlist based on track count
    if (tracks.Tracks.Length > 1)
    {
        var firstTrack = tracks.Tracks.First(x => !x.IsLiveStream && x.IsSeekable);

        // If player is not playing, start playing the first track
        if (player.State != PlayerState.Playing)
        {
            await player.PlayAsync(firstTrack);

            // Set volume if it's not set already
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

            // Set volume if it's not set already
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
