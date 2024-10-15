using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using mlean.Audio;

namespace mlean.Commands
{
    public class Skip(IAudioService audioService, DiscordSocketClient discordClient)
        : CommandBase(audioService)
    {
        [Command("skip", RunMode = RunMode.Async)]
        public async Task SkipAsync()
        {
            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }
            
            AudioManager.Initialize(AudioService, Context, discordClient);
        }
    }
}