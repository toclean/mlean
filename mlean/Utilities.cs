using System.Text;
using Discord;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Tracks;

namespace mlean;

public static class Utilities
{
    public static Embed ErrorEmbed(string message) =>
        new EmbedBuilder().WithTitle("Error").WithDescription(message).WithColor(Color.Red).WithCurrentTimestamp()
            .Build();

    public static Embed StatusEmbed(string message) =>
        new EmbedBuilder().WithTitle("Status").WithDescription(message).WithColor(Color.Gold).WithCurrentTimestamp()
            .Build();
    public static Embed CreateTrackEmbed(LavalinkTrack track, string action)
    {
        var embed = new EmbedBuilder()
            .WithTitle($"{action}: {track.Title}")
            .WithUrl(track.Uri?.ToString())
            .AddField("🎤 Author", track.Author, true)
            .AddField("⏱️ Duration", track.Duration.ToString(@"hh\:mm\:ss"), true)
            .AddField("📡 Source", track.SourceName ?? "Unknown", true)
            .WithColor(Color.Blue)
            .WithThumbnailUrl(track.ArtworkUri?.ToString())
            .WithCurrentTimestamp();

        return embed.Build();
    }

    public static string CreateEmojiSlider(float value)
    {
        int sliderLevel = (int)((value + 0.25) / 1.25 * 8); // Map to 0–8

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < sliderLevel; i++) sb.Append("🟩");
        for (int i = sliderLevel; i < 8; i++) sb.Append("⬛");

        return sb.ToString();
    }
}