using Discord;
using Discord.Commands;
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
        private static bool _isRepeat = false;
        private static bool _init = false;

        public static void Initialize(IAudioService audioService, SocketCommandContext context)
        {
            if (_init) return;
            _init = true;
            _context = context;
            _audioService = audioService;
            _audioService.TrackEnded += OnTrackEnded;
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
                await _context.Channel.SendMessageAsync(
                    embed: Utilities.CreateTrackEmbed(eventargs.Player.CurrentTrack, "Now Playing"));
            }
        }
    }
}