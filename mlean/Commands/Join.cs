using Discord;
using Discord.Commands;
using Lavalink4NET;

namespace mlean.Commands
{
    public class Join(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("join", RunMode = RunMode.Async, Summary = "Makes the bot join the current voice channel")]
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