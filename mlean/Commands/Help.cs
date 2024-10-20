using Discord;
using Discord.Commands;
using Lavalink4NET;

namespace mlean.Commands
{
    public class Help(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("help", RunMode = RunMode.Async)]
        public async Task HelpAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("📜 **Help & Commands**")
                .WithDescription($"Use `{Prefix}` as the command prefix for all commands.")
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp()
                .AddField("🎵 **Music Commands**",
                    $"`{Prefix}join` - Joins your current voice channel\n" +
                    $"`{Prefix}leave` - Leaves the voice channel\n" +
                    $"`{Prefix}play <query>` - Plays a song or adds to queue\n" +
                    $"`{Prefix}pause` - Pauses the current song\n" +
                    $"`{Prefix}resume` - Resumes the song\n" +
                    $"`{Prefix}stop` - Stops the music and clears the queue\n" +
                    $"`{Prefix}skip` - Skips the current track\n" +
                    $"`{Prefix}volume <level>` - Sets the volume (0-100%)")
                .AddField("🎛️ **Filter Commands**",
                    $"`{Prefix}show-filters` - Displays current filter status\n" +
                    $"`{Prefix}screw-it` - Toggles Screw-It mode\n" +
                    $"`{Prefix}timescale <speed> <pitch> <rate>` - Adjusts timescale filter\n" +
                    $"`{Prefix}vibrato <frequency> <depth>` - Applies or disables vibrato filter")
                .AddField("🎚️ **Equalizer Commands**",
                    $"`{Prefix}set-eq <band>:<gain>` - Adjusts the gain of a specific EQ band\n" +
                    $"`{Prefix}set-eq-all <gain>` - Sets all EQ bands to the same gain\n" +
                    $"`{Prefix}show-eq` - Displays the current EQ settings\n" +
                    $"`{Prefix}reset-equalizer` - Resets the equalizer to default")
                .AddField("⚙️ **Utility Commands**",
                    $"`{Prefix}help` - Displays this help message\n" +
                    $"`{Prefix}leave` - Makes the bot leave the voice channel");

            await ReplyAsync(embed: embed.Build());
        }
    }
}