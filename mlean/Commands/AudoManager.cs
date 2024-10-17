using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Tracks;

namespace mlean.Audio
{
    public static class AudioManager
    {
        private static IAudioService _audioService;
        private static SocketCommandContext _context;
        private static DiscordSocketClient _discordClient;
        private static bool _isRepeat = false;
        private static bool _init = false;

        public static void Initialize(IAudioService audioService, SocketCommandContext context, DiscordSocketClient discordClient)
        {
            if (audioService == null || context == null) throw new Exception("Failed to pass the correct information");
            if (_init) return;
            _init = true;
            _context = context;
            _audioService = audioService;
            _discordClient = discordClient;
            _audioService.TrackEnded += OnTrackEnded;
        }

        public static bool GetRepeat()
        {
            return _isRepeat;
        }

        public static void ToggleRepeat()
        {
            _isRepeat = !_isRepeat;
        }

        public static void UpdateTrackEvent()
        {
            // Unsubscribe previous handlers to prevent duplicate events
            _audioService.TrackEnded -= OnTrackEnded;

            // Re-subscribe the handler for the current player
            _audioService.TrackEnded += OnTrackEnded;
        }

        private static async Task OnTrackEnded(object sender, TrackEndedEventArgs eventargs)
        {
            if (eventargs.Reason == TrackEndReason.Finished)
            {
                var track = eventargs.Player.CurrentTrack;
                await UpdateBotStatusAsync(track);
                if (track == null) return;
                
                await _context.Channel.SendMessageAsync(
                    embed: Utilities.CreateTrackEmbed(track, "Now Playing"));
            }
        }

        public static async Task UpdateBotStatusAsync(LavalinkTrack? track = null)
        {
            await _discordClient.SetGameAsync(track != null ? $"🎵 {track.Title}" : "Ready for commands!",
                type: track != null ? ActivityType.Playing : ActivityType.Listening);
        }
    }
}