using Discord;
using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Tracks;

namespace mlean.Commands
{
    public abstract class CommandBase : ModuleBase<SocketCommandContext>
    {
        protected static readonly string Prefix = Environment.GetEnvironmentVariable("BOT_PREFIX") ?? "!";
        protected readonly IAudioService _audioService;
        protected readonly DiscordSocketClient _discordClient;

        protected CommandBase(IAudioService audioService, DiscordSocketClient discordClient)
        {
            _audioService = audioService;
            _discordClient = discordClient;
        }

        protected async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(bool join = false)
        {
            var options = new PlayerRetrieveOptions(
                ChannelBehavior: join ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

            var result = await _audioService.Players.RetrieveAsync(Context, PlayerFactory.Queued, options);
            if (result.IsSuccess) return result.Player;

            await ReplyAsync(embed: Utilities.ErrorEmbed(result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You're not in a voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "The bot isn't connected.",
                _ => "An unknown error occurred."
            }));

            return null;
        }

        protected async Task UpdateBotStatusAsync(LavalinkTrack? track = null)
        {
            await _discordClient.SetGameAsync(track != null ? $"🎵 {track.Title}" : "Ready for commands!",
                type: track != null ? ActivityType.Playing : ActivityType.Listening);
        }

        [Command("show-filters", RunMode = RunMode.Async)]
        public async Task ShowFiltersAsync()
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }

            var filters = player.Filters;
            var embed = new EmbedBuilder()
                .WithTitle("🎛️ Filter & EQ Status")
                .WithColor(Color.Teal)
                .WithCurrentTimestamp()
                .AddField("🔊 Low-Pass Filter", filters.LowPass != null ? "ON ✅" : "OFF ❌", true)
                .AddField("🔄 Rotation", filters.Rotation != null ? "ON ✅" : "OFF ❌", true)
                .AddField("⏩ Timescale", filters.Timescale != null ? "ON ✅" : "OFF ❌", true)
                .AddField("🌊 Vibrato", filters.Vibrato != null ? "ON ✅" : "OFF ❌", true);

            var eq = filters.Equalizer?.Equalizer.ToArray() ?? new float[15];
            for (int i = 0; i < eq.Length; i++)
            {
                embed.AddField($"🎚️ EQ Band {i}", $"Gain: {eq[i]:0.00}", true);
            }

            await ReplyAsync(embed: embed.Build());
        }
        
        [Command("show-eq", RunMode = RunMode.Async)]
        public async Task ShowEqualizerAsync()
        {
            var player = await GetPlayerAsync();
            if (player == null || player.Filters.Equalizer == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("No EQ settings found."));
                return;
            }

            var eq = player.Filters.Equalizer.Equalizer.ToArray();
            var embed = new EmbedBuilder().WithTitle("🎛️ Equalizer Settings").WithColor(Color.Blue).WithCurrentTimestamp();

            for (int i = 0; i < eq.Length; i++)
            {
                embed.AddField($"Band {i}", Utilities.CreateEmojiSlider(eq[i]), inline: true);
            }

            await ReplyAsync(embed: embed.Build());
        }
    }
}