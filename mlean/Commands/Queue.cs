using Discord.Commands;
using Lavalink4NET;

namespace mlean.Commands
{
    public class Queue(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("queue", RunMode = RunMode.Async)]
        public async Task JoinAsync()
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await Context.Channel.SendMessageAsync(embed: Utilities.ErrorEmbed("Player was not initialized"));
                return;
            }

            var queue = player.Queue;

            await Context.Channel.SendMessageAsync(embed: Utilities.CreateQueueList(queue));
        }
    }
}