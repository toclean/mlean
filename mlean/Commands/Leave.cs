using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;
using mlean.Audio;

namespace mlean.Commands
{
    public class Leave(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveAsync()
        {
            var player = await GetPlayerAsync();
            if (player != null) await player.DisconnectAsync();

            await AudioManager.UpdateBotStatusAsync();
            await ReplyAsync(embed: Utilities.StatusEmbed("Left the voice channel."));
        }
    }
}