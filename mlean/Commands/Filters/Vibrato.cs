using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Filters;

namespace mlean.Commands.Filters
{
    public class Vibrato(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("vibrato", RunMode = RunMode.Async)]
        public async Task VibratoAsync(float? frequency = null, float? depth = null)
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }

            if (!frequency.HasValue || !depth.HasValue)
            {
                player.Filters.Vibrato = null;
                await player.Filters.CommitAsync();

                await ReplyAsync(embed: Utilities.StatusEmbed("🌊 Vibrato Disabled."));
            }
            else
            {
                if (frequency.Value <= 0 || depth.Value <= 0)
                {
                    await ReplyAsync("Both frequency and depth must be greater than 0.");
                    return;
                }

                player.Filters.Vibrato = new VibratoFilterOptions(frequency.Value, depth.Value);
                await player.Filters.CommitAsync();

                await ReplyAsync(embed: Utilities.StatusEmbed(
                    $"🌊 Vibrato Enabled - Frequency: {frequency.Value}, Depth: {depth.Value}."));
            }

            await ShowFiltersAsync();
        }
    }
}