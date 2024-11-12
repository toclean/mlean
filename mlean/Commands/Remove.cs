using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;

namespace mlean.Commands
{
    public class Remove(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("Remove", RunMode = RunMode.Async, Summary = "Removes song from queue at index")]
        public async Task RemoveAsync([Remainder] int index)
        {
            var player = await GetPlayerAsync();
            if (player == null) return;

            try
            {
                var trackToRemove = player.Queue[index];
                var wasSuccessful = await player.Queue.RemoveAsync(trackToRemove);
                if (wasSuccessful)
                {
                    await Context.Channel.SendMessageAsync(
                        embed: Utilities.StatusEmbed($"Removed {trackToRemove?.Track?.Title}!"));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(
                        embed: Utilities.ErrorEmbed($"Could not remove {trackToRemove.Track?.Title}."));
                }
            }
            catch
            {
                await Context.Channel.SendMessageAsync(embed: Utilities.ErrorEmbed("Unable to remove song."));
            }
        }
    }
}