using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Filters;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using ContextType = Discord.Interactions.ContextType;

namespace mlean;

[Discord.Interactions.RequireContext(ContextType.Guild)]
public class AudioModule : ModuleBase<SocketCommandContext>
{
    private readonly IAudioService _audioService;
    private readonly DiscordSocketClient _discordClient;
    private static readonly string Prefix = Environment.GetEnvironmentVariable("BOT_PREFIX") ?? "!";

    public AudioModule(IAudioService audioService, DiscordSocketClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    [Command("join", RunMode = RunMode.Async)]
    public async Task JoinAsync()
    {
        await GetPlayerAsync(true);
        var embed = new EmbedBuilder()
            .WithTitle("Joined Voice Channel")
            .WithDescription($"{Context.User.Username} summoned me to the voice channel!")
            .WithColor(Color.Green)
            .WithCurrentTimestamp()
            .Build();

        await ReplyAsync(embed: embed);
    }

    [Command("leave", RunMode = RunMode.Async)]
    public async Task LeaveAsync()
    {
        var player = await GetPlayerAsync();
        if (player != null) await player.DisconnectAsync();

        await UpdateBotStatusAsync();
        await ReplyAsync(embed: StatusEmbed("Left the voice channel."));
    }

    [Command("help", RunMode = RunMode.Async)]
    public async Task HelpAsync()
    {
        var embed = new EmbedBuilder()
            .WithTitle("📜 **Help & Commands**")
            .WithDescription($"Use `{Prefix}` as the command prefix.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp()
            .AddField("🎵 **Music Commands**", 
                $"`{Prefix}join` - Joins your current voice channel\n" +
                $"`{Prefix}leave` - Leaves the voice channel\n" +
                $"`{Prefix}play <query>` - Plays a song\n" +
                $"`{Prefix}pause` - Pauses the current song\n" +
                $"`{Prefix}resume` - Resumes the song\n" +
                $"`{Prefix}stop` - Stops the music\n" +
                $"`{Prefix}skip` - Skips the song")
            .AddField("🎛️ **Filter Commands**", 
                $"`{Prefix}show-filters` - Displays current filters\n" +
                $"`{Prefix}screw-it` - Toggles Screw-It mode\n" +
                $"`{Prefix}timescale <speed> <pitch> <rate>` - Timescale filter\n" +
                $"`{Prefix}vibrato <frequency> <depth>` - Vibrato filter")
            .AddField("🎚️ **Equalizer Commands**", 
                $"`{Prefix}set-eq <band>:<gain>` - Adjusts EQ band\n" +
                $"`{Prefix}set-eq-all <gain>` - Set all EQ bands\n" +
                $"`{Prefix}reset-equalizer` - Resets EQ")
            .AddField("⚙️ **Utility Commands**", 
                $"`{Prefix}help` - Shows this help message");

        await ReplyAsync(embed: embed.Build());
    }
    
    [Command("skip", RunMode = RunMode.Async)]
    public async Task SkipAsync()
    {
        var player = await GetPlayerAsync();
        if (player == null)
        {
            await ReplyAsync(embed: ErrorEmbed("Player not found."));
            return;
        }

        if (player.Queue.Count > 0)
        {
            await player.SkipAsync();
            await UpdateBotStatusAsync(player.CurrentTrack);
            await ReplyAsync(embed: StatusEmbed("⏩ Skipped to the next track."));
        }
        else
        {
            await player.StopAsync();
            await UpdateBotStatusAsync();
            await ReplyAsync(embed: StatusEmbed("🛑 No more tracks in the queue. Stopped playback."));
        }
    }

    [Command("screw-it", RunMode = RunMode.Async)]
    public async Task ScrewItAsync()
    {
        var player = await GetPlayerAsync();
        if (player == null)
        {
            await ReplyAsync("Player not found.");
            return;
        }

        bool isScrewItActive = player.Filters.Timescale != null;
        player.Filters.Timescale = isScrewItActive ? null : new TimescaleFilterOptions(0.8f, 0.7f, 1.0f);
        await player.Filters.CommitAsync();

        await ReplyAsync(embed: StatusEmbed(isScrewItActive ? 
            "🚫 Screw-It Mode Deactivated" : 
            "🔧 Screw-It Mode Activated with Speed 0.8, Pitch 0.7, Rate 1.0"));

        await ShowFiltersAsync();
    }

    [Command("show-filters", RunMode = RunMode.Async)]
    public async Task ShowFiltersAsync()
    {
        var player = await GetPlayerAsync();
        if (player == null)
        {
            await ReplyAsync(embed: ErrorEmbed("Player not found."));
            return;
        }

        var filters = player.Filters;
        var embed = new EmbedBuilder()
            .WithTitle("🎛️ Filter Status")
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .AddField("🔊 Low-Pass Filter", filters.LowPass != null ? "ON ✅" : "OFF ❌", true)
            .AddField("🔄 Rotation", filters.Rotation != null ? "ON ✅" : "OFF ❌", true)
            .AddField("⏩ Timescale", filters.Timescale != null ? "ON ✅" : "OFF ❌", true)
            .AddField("🌊 Vibrato", filters.Vibrato != null ? "ON ✅" : "OFF ❌", true);

        await ReplyAsync(embed: embed.Build());
    }

    [Command("play", RunMode = RunMode.Async)]
    public async Task PlayAsync([Remainder] string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            await ReplyAsync(embed: ErrorEmbed("Provide a search query."));
            return;
        }

        var tracks = await _audioService.Tracks.LoadTracksAsync(searchQuery, TrackSearchMode.YouTube);
        if (!tracks.HasMatches)
        {
            await ReplyAsync(embed: ErrorEmbed($"No results for `{searchQuery}`."));
            return;
        }

        var track = tracks.Tracks.FirstOrDefault(x => !x.IsLiveStream && x.IsSeekable);
        var player = await GetPlayerAsync(true);
        if (player == null) return;

        if (player.State == PlayerState.Playing)
        {
            await player.Queue.AddAsync(new TrackQueueItem(track));
            await ReplyAsync(embed: CreateTrackEmbed(track, "Added to Queue"));
        }
        else
        {
            await player.PlayAsync(track);
            await UpdateBotStatusAsync(track);
            await ReplyAsync(embed: CreateTrackEmbed(track, "Now Playing"));
        }
    }

    private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(bool join = false)
    {
        var options = new PlayerRetrieveOptions(
            ChannelBehavior: join ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var result = await _audioService.Players.RetrieveAsync(Context, PlayerFactory.Queued, options);
        if (result.IsSuccess) return result.Player;

        await ReplyAsync(embed: ErrorEmbed(result.Status switch
        {
            PlayerRetrieveStatus.UserNotInVoiceChannel => "You're not in a voice channel.",
            PlayerRetrieveStatus.BotNotConnected => "The bot isn't connected.",
            _ => "An unknown error occurred."
        }));

        return null;
    }

    private async Task UpdateBotStatusAsync(LavalinkTrack? track = null)
    {
        if (track != null)
        {
            await _discordClient.SetGameAsync($"🎵 {track.Title}", type: ActivityType.Playing);
        }
        else
        {
            await _discordClient.SetGameAsync("Ready for commands!", type: ActivityType.Listening);
        }
    }

    private Embed ErrorEmbed(string message) =>
        new EmbedBuilder()
            .WithTitle("Error")
            .WithDescription(message)
            .WithColor(Color.Red)
            .WithCurrentTimestamp()
            .Build();

    private Embed StatusEmbed(string message) =>
        new EmbedBuilder()
            .WithTitle("Status")
            .WithDescription(message)
            .WithColor(Color.Gold)
            .WithCurrentTimestamp()
            .Build();

    private Embed CreateTrackEmbed(LavalinkTrack track, string action) =>
        new EmbedBuilder()
            .WithTitle($"{action}: {track.Title}")
            .WithUrl(track.Uri.ToString())
            .AddField("Author", track.Author ?? "Unknown", true)
            .AddField("Duration", track.Duration.ToString(@"hh\:mm\:ss"), true)
            .WithColor(Color.Blue)
            .WithThumbnailUrl(track.ArtworkUri?.ToString())
            .WithCurrentTimestamp()
            .Build();
}
