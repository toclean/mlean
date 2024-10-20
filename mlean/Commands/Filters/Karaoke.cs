using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Filters;

namespace mlean.Commands
{
    public class Karaoke(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("karaoke", RunMode = RunMode.Async)]
        public async Task KaraokeAsync(int? level, int? monoLevel, int? filterBand, int? filterWidth)
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }

            if (!level.HasValue || !monoLevel.HasValue || !filterBand.HasValue || !filterWidth.HasValue)
            {
                player.Filters.Karaoke = null;
                await player.Filters.CommitAsync();

                await ReplyAsync(embed: Utilities.StatusEmbed("🎤 Karaoke Disabled."));
            }
            else
            {
                player.Filters.Karaoke = new KaraokeFilterOptions(level, monoLevel, filterBand, filterWidth);
                await player.Filters.CommitAsync();

                await ReplyAsync(embed: Utilities.StatusEmbed(
                    $"🎤 Karaoke Enabled - Level: {level.Value}, MonoLevel: {monoLevel.Value}, FilterBand: {filterBand.Value}, FilterWidth: {filterWidth.Value}"));
            }

            await ShowFiltersAsync();
        }
    }
}