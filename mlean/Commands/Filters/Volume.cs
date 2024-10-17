using Discord;
using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Filters;

namespace mlean.Commands.Eq
{
    public class Volume(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("volume", RunMode = RunMode.Async)]
        public async Task SetVolumeAsync(float volumePercent)
        {
            if (volumePercent < 0 || volumePercent > 100)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Please provide a volume level between 0 and 100."));
                return;
            }

            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }
            
            await player.SetVolumeAsync(volumePercent / 1000);

            await ReplyAsync(embed: Utilities.StatusEmbed($"🔊 Volume set to {volumePercent}%."));
        }

    }
}