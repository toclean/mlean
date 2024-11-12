using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;

namespace mlean.Commands
{
    public class Skip(IAudioService audioService, DiscordSocketClient discordClient)
        : CommandBase(audioService)
    {
        [Command("skip", RunMode = RunMode.Async, Summary = "Skips the current song if one is playing")]
        public async Task SkipAsync()
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }
            
            AudioManager.Initialize(AudioService, Context, discordClient);
            await player.SkipAsync();
        }
    }
}