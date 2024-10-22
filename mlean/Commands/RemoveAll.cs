using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;

namespace mlean.Commands
{
    public class RemoveAll(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("RemoveAll", RunMode = RunMode.Async)]
        public async Task RemoveAllAsync()
        {
            
            var player = await GetPlayerAsync();
            if (player == null) return;
            
            try
            {
                await player.Queue.RemoveAllAsync(x => x.Track != null);
                await Context.Channel.SendMessageAsync(
                    embed: Utilities.StatusEmbed($"Removed all tracks in queue!"));
            }
            catch (Exception)
            {
                await Context.Channel.SendMessageAsync(
                    embed: Utilities.ErrorEmbed($"Could not remove all tracks in queue"));
            }
        }
    }
}
