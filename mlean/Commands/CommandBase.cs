using System.Text;
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
        protected async Task ShowEqualizerAsync()
        {
            var player = await GetPlayerAsync();
            if (player == null || player.Filters.Equalizer == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("No EQ settings found."));
                return;
            }

            var eq = player.Filters.Equalizer.Equalizer.ToArray();
            var embed = new EmbedBuilder()
                .WithTitle("🎛️ Equalizer Settings")
                .WithColor(Color.Blue)
                .WithCurrentTimestamp();

            // Build a vertical representation with a fixed spacing for each EQ band
            string[] bars = new string[8];
            for (int i = 0; i < bars.Length; i++) bars[i] = "";

            // Generate the vertical representation for each band
            for (int i = 0; i < eq.Length; i++)
            {
                int levels = (int)((eq[i] + 0.25f) * 8); // Map gain to slider (0-8)
                for (int j = 0; j < 8; j++)
                {
                    if (j < 8 - levels)
                        bars[j] += "⬛  ";  // Add empty block with consistent spacing
                    else
                        bars[j] += "🟩  ";  // Add filled block with consistent spacing
                }
            }

            // Create the vertical EQ representation as a string
            StringBuilder eqRepresentation = new StringBuilder();
            foreach (string bar in bars)
            {
                eqRepresentation.AppendLine(bar.TrimEnd());
            }

            embed.AddField("EQ", $"```{eqRepresentation.ToString()}```");

            await ReplyAsync(embed: embed.Build());
        }
    }
}