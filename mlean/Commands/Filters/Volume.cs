using Discord.Commands;
using Lavalink4NET;

namespace mlean.Commands.Filters
{
    public class Volume(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("volume", RunMode = RunMode.Async)]
        public async Task SetVolumeAsync(float volumePercent)
        {
            if (volumePercent < 0 || volumePercent > 150)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Please provide a volume level between 0 and 150."));
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