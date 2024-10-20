using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Filters;

namespace mlean.Commands.Filters
{
    public class Reverb(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("reverb", RunMode = RunMode.Async)]
        public async Task ReverbAsync(
            float? level = null,
            float? monoLevel = null,
            float? filterBand = null,
            float? filterWidth = null)
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }

            if (!level.HasValue || !monoLevel.HasValue ||
                !filterBand.HasValue || !filterWidth.HasValue)
            {
                // Disable reverb effect
                player.Filters.Karaoke = null;
                await player.Filters.CommitAsync();

                await ReplyAsync(embed: Utilities.StatusEmbed("🎶 Reverb effect disabled."));
            }
            else
            {
                // Validate input values
                if (level < 0 || monoLevel < 0 || filterBand < 0 || filterWidth < 0)
                {
                    await ReplyAsync(embed: Utilities.ErrorEmbed("All values must be non-negative."));
                    return;
                }

                // Apply reverb effect through the karaoke filter
                player.Filters.Karaoke = new KaraokeFilterOptions(
                    Level: level.Value,
                    MonoLevel: monoLevel.Value,
                    FilterBand: filterBand.Value,
                    FilterWidth: filterWidth.Value
                );

                await player.Filters.CommitAsync();

                await ReplyAsync(embed: Utilities.StatusEmbed(
                    $"🎶 Reverb applied: Level {level}, Mono {monoLevel}, Band {filterBand}, Width {filterWidth}."));
            }

            await ShowFiltersAsync(); // Display updated filter settings
        }
    }
}