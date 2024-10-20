using Lavalink4NET.Players;
using Lavalink4NET.Tracks;

public class TrackQueueItem : ITrackQueueItem
{
    public TrackReference Reference { get; }
    public LavalinkTrack Track { get; }

    public TrackQueueItem(LavalinkTrack track)
    {
        Track = track;
        Reference = new TrackReference(track);
    }
}