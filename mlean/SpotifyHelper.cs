using SpotifyAPI.Web;
using System.Threading.Tasks;

public static class SpotifyHelper
{
    private static readonly string ClientId = "49e4155c8b504ddd9f4afe65016c1708"; // Replace with your Spotify Client ID
    private static readonly string ClientSecret = "f1ce2dcfc4624bc5a19ee8f6e5fc403c"; // Replace with your Spotify Client Secret

    private static SpotifyClient _spotifyClient;

    // Initialize the Spotify client
    private static async Task InitializeSpotifyClientAsync()
    {
        if (_spotifyClient != null) return;

        // Create a new config with your credentials
        var config = SpotifyClientConfig.CreateDefault();
        var request = new ClientCredentialsRequest(ClientId, ClientSecret);
        var response = await new OAuthClient(config).RequestToken(request);

        // Create the Spotify client
        _spotifyClient = new SpotifyClient(config.WithToken(response.AccessToken));
    }

    // Fetch track details from a Spotify link
    public static async Task<SpotifyTrack> GetTrackDetailsAsync(string spotifyLink)
    {
        // Initialize the Spotify client
        await InitializeSpotifyClientAsync();

        // Extract the track ID from the Spotify link
        var trackId = ExtractTrackIdFromLink(spotifyLink);
        if (string.IsNullOrEmpty(trackId))
        {
            return null;
        }

        // Fetch the track details from the Spotify API
        var track = await _spotifyClient.Tracks.Get(trackId);
        if (track == null)
        {
            return null;
        }

        // Extract the track name and artist
        var trackName = track.Name;
        var artistName = track.Artists.Count > 0 ? track.Artists[0].Name : "Unknown Artist";

        return new SpotifyTrack
        {
            Name = trackName,
            Artist = artistName
        };
    }

    // Extract the track ID from a Spotify link
    private static string ExtractTrackIdFromLink(string spotifyLink)
    {
        // Example link: https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT
        var uriParts = spotifyLink.Split('/');
        if (uriParts.Length < 5 || uriParts[3] != "track")
        {
            return null;
        }

        return uriParts[4];
    }
}

public class SpotifyTrack
{
    public string Name { get; set; }
    public string Artist { get; set; }
}