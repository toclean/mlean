using Discord;
using Discord.Commands;
using HtmlAgilityPack;
using Lavalink4NET;

namespace mlean.Commands
{
    public class PlayerCounts(IAudioService audioService)
        : CommandBase(audioService)
    {
        private static readonly HttpClient client = new HttpClient();
        
        [Command("player-counts", Summary = "Gets the player count of Dark and Darker")]
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
        
        // Function to fetch Dark and Darker player count from Steam Charts
        private async Task<string> GetDarkAndDarkerPlayerCountAsync()
        {
            string url = "https://steamcharts.com/app/2016590"; // Dark and Darker on Steam Charts
            var response = await client.GetStringAsync(url);

            // Load the response into HtmlDocument
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Use XPath to locate the current player count (simplified)
            var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='app-stat']/span[@class='num']");
        
            if (node != null)
            {
                return node.InnerText.Trim(); // Return the player count
            }
            return "Unable to fetch";
        }
    }
}