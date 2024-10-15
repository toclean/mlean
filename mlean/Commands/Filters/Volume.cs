using Discord;
using Discord.Commands;
using Lavalink4NET;
using Discord.WebSocket;
using Lavalink4NET.Filters;

namespace mlean.Commands.Eq
{
    public class Volume(IAudioService audioService, DiscordSocketClient discordClient)
        : CommandBase(audioService, discordClient)
    {
        [Command("volume", RunMode = RunMode.Async)]
        public async Task SetVolumeAsync(int volume)
        {
            if (volume < 0 || volume > 100)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Please provide a volume level between 0 and 100."));
                return;
            }

            var player = await GetPlayerAsync();
            if (player == null)
            {
                await ReplyAsync(embed: Utilities.ErrorEmbed("Player not found."));
                return;
            }

            // Set the player's volume (Lavalink expects a value between 0 and 1000)
            await player.SetVolumeAsync(volume);

            await ReplyAsync(embed: Utilities.StatusEmbed($"🔊 Volume set to {volume}%"));
        }
    }
}