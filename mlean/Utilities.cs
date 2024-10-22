using System.Text;
using Discord;
using Lavalink4NET.Players.Queued;
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

    public static List<Embed> CreateQueueList(ITrackQueue queue)
    {
        const int maxFieldsPerPage = 10;
        var embeds = new List<Embed>();
        var embedBuilder = new EmbedBuilder()
            .WithTitle("🎸 Song Queue 🎷")
            .WithColor(Color.Purple)
            .WithCurrentTimestamp();

        int fieldCount = 0;

        foreach (var queueItem in queue)
        {
            var track = queueItem.Track;
            if (track == null) continue;

            embedBuilder.AddField(track.Title, track.Author);
            fieldCount++;

            if (fieldCount >= maxFieldsPerPage)
            {
                embeds.Add(embedBuilder.Build());
                embedBuilder = new EmbedBuilder()
                    .WithTitle("🎸 Song Queue 🎷 (Continued)")
                    .WithColor(Color.Purple)
                    .WithCurrentTimestamp();
                fieldCount = 0;
            }
        }

        if (fieldCount > 0)
        {
            embeds.Add(embedBuilder.Build());
        }

        return embeds;
    }
    
    public static Embed CreatePlaylistEmbed(int trackCount, string message)
    {
        var embed = new EmbedBuilder()
            .WithTitle("Playlist Added")
            .WithDescription($"{trackCount} tracks added to the queue.")
            .WithFooter(message)
            .WithColor(Color.Green)
            .Build();

        return embed;
    }

}