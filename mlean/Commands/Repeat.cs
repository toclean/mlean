using Discord.Commands;
using Lavalink4NET;

namespace mlean.Commands
{
    public class Repeat(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("repeat", RunMode = RunMode.Async, Summary = "Toggles the repeat mode on/off")]
        public async Task RepeatAsync()
        {
            AudioManager.ToggleRepeat();
            var text = AudioManager.GetRepeat() ? "ON" : "OFF";
            await ReplyAsync(embed: Utilities.StatusEmbed($"Repeat is now {text}"));
        }
    }
}