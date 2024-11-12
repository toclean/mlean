using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Filters;

namespace mlean.Commands.Filters
{
    public class ScrewIt(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("screw-it", RunMode = RunMode.Async, Summary = "Applies the screw-it (Houston Mode) filter")]
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

            await ReplyAsync(embed: Utilities.StatusEmbed(isScrewItActive
                ? "🚫 Screw-It Mode Deactivated"
                : "🔧 Screw-It Mode Activated with Speed 0.8, Pitch 0.7, Rate 1.0"));

            await ShowFiltersAsync();
        }
    }
}