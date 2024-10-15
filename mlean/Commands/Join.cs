using Discord;
using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.Players;
using Discord.WebSocket;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players.Queued;

namespace mlean.Commands
{
    public class Join(IAudioService audioService, DiscordSocketClient discordClient)
        : CommandBase(audioService, discordClient)
    {
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinAsync()
        {
            await GetPlayerAsync(true);
            var embed = new EmbedBuilder()
                .WithTitle("Joined Voice Channel")
                .WithDescription($"{Context.User.Username} summoned me to the voice channel!")
                .WithColor(Color.Green)
                .WithCurrentTimestamp()
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}