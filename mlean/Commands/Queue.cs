using Discord.Commands;
using Lavalink4NET;
using Discord;
using Lavalink4NET.Players;
using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Interactions;
using RunMode = Discord.Commands.RunMode;

namespace mlean.Commands
{
    public class Queue : CommandBase
    {
        // Use a ConcurrentDictionary to ensure thread safety
        private static ConcurrentDictionary<ulong, int> _queuePageTracker = new ConcurrentDictionary<ulong, int>();

        public Queue(IAudioService audioService) : base(audioService) { }

        [Command("queue", RunMode = RunMode.Async)]
        public async Task QueueAsync()
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await Context.Channel.SendMessageAsync(embed: Utilities.ErrorEmbed("Player was not initialized"));
                return;
            }

            var queue = player.Queue;
            var embeds = Utilities.CreateQueueListPaginated(queue);

            if (embeds.Count == 0)
            {
                await Context.Channel.SendMessageAsync(embed: Utilities.ErrorEmbed("The queue is empty."));
                return;
            }

            // Set initial page to 0 for this guild
            _queuePageTracker[Context.Guild.Id] = 0;

            var currentPage = _queuePageTracker[Context.Guild.Id];
            var message = await Context.Channel.SendMessageAsync(embed: embeds[currentPage], components: GetPaginationComponents(currentPage, embeds.Count));
        }

        private MessageComponent GetPaginationComponents(int currentPage, int totalPages)
        {
            var builder = new ComponentBuilder();

            // Embed current page in the button IDs to keep track across interactions
            builder.WithButton("Previous", $"queue_prev_{currentPage}", disabled: currentPage == 0);
            builder.WithButton("Next", $"queue_next_{currentPage}", disabled: currentPage == totalPages - 1);

            return builder.Build();
        }

        [ComponentInteraction("queue_prev")]
        public async Task QueuePreviousAsync(SocketMessageComponent component)
        {
            var guildId = component.GuildId.Value;

            var currentPage = _queuePageTracker.GetValueOrDefault(guildId);
            if (currentPage == 0)
            {
                await component.RespondAsync(embed: Utilities.ErrorEmbed("You are already on the first page."), ephemeral: true);
                return;
            }

            _queuePageTracker[guildId] = --currentPage;

            var player = await GetPlayerAsync();
            if (player == null)
            {
                await component.RespondAsync(embed: Utilities.ErrorEmbed("Player was not initialized"), ephemeral: true);
                return;
            }

            var queue = player.Queue;
            var embeds = Utilities.CreateQueueListPaginated(queue);

            await component.UpdateAsync(msg =>
            {
                msg.Embed = embeds[currentPage];
                msg.Components = GetPaginationComponents(currentPage, embeds.Count);
            });
        }

        [ComponentInteraction("queue_next")]
        public async Task QueueNextAsync(SocketMessageComponent component)
        {
            var guildId = component.GuildId.Value;

            var currentPage = _queuePageTracker.GetValueOrDefault(guildId);
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await component.RespondAsync(embed: Utilities.ErrorEmbed("Player was not initialized"), ephemeral: true);
                return;
            }

            var queue = player.Queue;
            var embeds = Utilities.CreateQueueListPaginated(queue);

            if (currentPage >= embeds.Count - 1)
            {
                await component.RespondAsync(embed: Utilities.ErrorEmbed("You are already on the last page."), ephemeral: true);
                return;
            }

            _queuePageTracker[guildId] = ++currentPage;

            await component.UpdateAsync(msg =>
            {
                msg.Embed = embeds[currentPage];
                msg.Components = GetPaginationComponents(currentPage, embeds.Count);
            });
        }

    }
}
