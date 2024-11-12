using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Filters;

namespace mlean.Commands.Filters.Eq
{
    public class ResetEq(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("reset-eq", RunMode = RunMode.Async, Summary = "Resets the eq settings")]
        public async Task ResetEqualizerAsync()
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }

            var builder = Equalizer.CreateBuilder();
            for (int i = 0; i < Equalizer.Bands; i++) builder[i] = 0.0f;

            player.Filters.Equalizer = new EqualizerFilterOptions(builder.Build());
            await player.Filters.CommitAsync();
            await ReplyAsync("Equalizer reset to default.");
        }
    }
}