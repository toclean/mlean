﻿using Discord;
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
            _audioService.TrackStarted += OnTrackStarted;
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
            _audioService.TrackStarted -= OnTrackStarted;

            // Re-subscribe the handler for the current player
            _audioService.TrackEnded += OnTrackEnded;
            _audioService.TrackStarted += OnTrackStarted;
        }

        private static async Task OnTrackStarted(object sender, TrackStartedEventArgs eventargs)
        {
            await UpdateBotStatusAsync(eventargs.Track);
            await _context.Channel.SendMessageAsync(embed: Utilities.CreateTrackEmbed(eventargs.Track, "Now Playing"));
        }

        private static async Task OnTrackEnded(object sender, TrackEndedEventArgs eventargs)
        {
            switch (eventargs.Reason)
            {
                case TrackEndReason.Finished:
                {
                    var track = eventargs.Player.CurrentTrack;
                    await UpdateBotStatusAsync(track);
                    break;
                }
                case TrackEndReason.Replaced:
                {
                    var track = eventargs.Player.CurrentTrack;
                    var embed = track == null
                        ? Utilities.StatusEmbed("🛑 No more tracks. Stopped playback.")
                        : Utilities.StatusEmbed("⏩ Skipped to the next track.");
                
                    await _context.Channel.SendMessageAsync(embed: embed);
                    await UpdateBotStatusAsync(track);
                    break;
                }
                case TrackEndReason.LoadFailed:
                    break;
                case TrackEndReason.Stopped:
                    break;
                case TrackEndReason.Cleanup:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static async Task UpdateBotStatusAsync(LavalinkTrack? track = null)
        {
            await _discordClient.SetGameAsync(track != null ? $"🎵 {track.Title}" : "Ready for commands!",
                type: track != null ? ActivityType.Playing : ActivityType.Listening);
        }
    }
}