using Discord;
using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Rest.Entities.Tracks;
using mlean.Audio;

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

            var track = tracks.Tracks.First(x => !x.IsLiveStream && x.IsSeekable);
            var player = await GetPlayerAsync(true);
            
            AudioManager.Initialize(AudioService, Context, discordClient);

            if (player == null) return;

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