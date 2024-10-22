using Discord;
using Discord.Commands;
using Lavalink4NET;

namespace mlean.Commands
{
    public class PlayerCounts(IAudioService audioService)
        : CommandBase(audioService)
    {
        private static readonly HttpClient client = new HttpClient();
        
        [Command("playercounts")]
        public async Task GetPlayerCountsAsync()
        {
            string darkAndDarkerPlayers = await GetDarkAndDarkerPlayerCountAsync();
            // string tarkovPlayers = await GetTarkovPlayerCountAsync();

            var embed = new EmbedBuilder()
                .WithTitle("Current Player Counts")
                .AddField("Dark and Darker", darkAndDarkerPlayers, true)
                // .AddField("Escape from Tarkov", tarkovPlayers, true)
                .WithColor(Color.Blue)
                .Build();

            await ReplyAsync(embed: embed);
        }
        
        // Function to fetch Dark and Darker player count
        private async Task<string> GetDarkAndDarkerPlayerCountAsync()
        {
            string url = "https://steamcharts.com/app/2016590"; // Dark and Darker on Steam Charts
            var response = await client.GetStringAsync(url);

            // Parsing the HTML for the current player count (simplified)
            string marker = "data: [";
            int start = response.IndexOf(marker) + marker.Length;
            int end = response.IndexOf(']', start);
            string currentPlayers = response.Substring(start, end - start).Split(',')[0].Trim();

            return currentPlayers;
        }

        // Function to fetch Escape from Tarkov player count
        private async Task<string> GetTarkovPlayerCountAsync()
        {
            // As Tarkov does not have a Steam listing, an API or custom source might be necessary
            // Replace with a valid API URL or web scraping logic
            string tarkovUrl = "https://some-api-or-website-for-tarkov-data.com"; // Placeholder URL
            var response = await client.GetStringAsync(tarkovUrl);

            // For now, returning a mock value; customize this as per the data source
            string currentPlayers = "583,000"; // Placeholder value, replace with real-time data

            return currentPlayers;
        }
    }
}