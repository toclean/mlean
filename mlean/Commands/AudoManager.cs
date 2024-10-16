using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Tracks;

namespace mlean.Audio
{
    public static class AudioManager
    {
        private static IAudioService _audioService;
        private static bool _isRepeat = false;

        public static void Initialize(IAudioService audioService)
        {
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
            if (_isRepeat && eventargs.Reason == TrackEndReason.Finished)
            {
                await eventargs.Player.PlayAsync(eventargs.Track);
            }
        }
    }
}