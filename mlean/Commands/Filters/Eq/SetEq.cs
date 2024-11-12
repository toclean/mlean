using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Filters;

namespace mlean.Commands.Filters.Eq
{
    public class SetEq(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("set-eq", RunMode = RunMode.Async, Summary = "Adjusts the eq settings")]
        public async Task SetEqualizerBandsAsync(params string[] bandGainPairs)
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }

            var builder = Equalizer.CreateBuilder(player.Filters.Equalizer?.Equalizer ?? Equalizer.Default);

            foreach (var pair in bandGainPairs)
            {
                var split = pair.Split(':');
                if (split.Length != 2 ||
                    !int.TryParse(split[0], out int band) ||
                    !float.TryParse(split[1], out float gain) ||
                    band < 0 || band >= Equalizer.Bands ||
                    gain < -0.25f || gain > 1.0f)
                {
                    await ReplyAsync($"Invalid format: `{pair}`. Use `band:gain` format.");
                    return;
                }

                builder[band] = gain;
            }

            player.Filters.Equalizer = new EqualizerFilterOptions(builder.Build());
            await player.Filters.CommitAsync();
            await ShowEqualizerAsync();
        }
    }
}