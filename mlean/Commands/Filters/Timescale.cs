using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Filters;

namespace mlean.Commands
{
    public class Timescale(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("timescale", RunMode = RunMode.Async)]
        public async Task TimeScaleAsync(float? speed = null, float? pitch = null, float? rate = null)
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }

            // If no parameters are provided, disable the timescale filter.
            if (!speed.HasValue || !pitch.HasValue || !rate.HasValue)
            {
                player.Filters.Timescale = null;
                await player.Filters.CommitAsync();

                await ReplyAsync(embed: Utilities.StatusEmbed("⏸️ Timescale filter has been disabled."));
            }
            else
            {
                // Validate input values.
                if (speed <= 0 || pitch <= 0 || rate <= 0)
                {
                    await ReplyAsync("All values for speed, pitch, and rate must be greater than 0.");
                    return;
                }

                // Apply the timescale filter.
                player.Filters.Timescale = new TimescaleFilterOptions(
                    Speed: speed.Value,
                    Pitch: pitch.Value,
                    Rate: rate.Value
                );
                await player.Filters.CommitAsync();

                await ReplyAsync(embed: Utilities.StatusEmbed(
                    $"⏩ Timescale Activated - Speed: {speed.Value}, Pitch: {pitch.Value}, Rate: {rate.Value}."));
            }

            // Display the updated filter status.
            await ShowFiltersAsync();
        }
    }
}