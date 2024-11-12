using Discord;
using Discord.Commands;
using Lavalink4NET;

namespace mlean.Commands
{
    public class Help(IAudioService audioService)
        : CommandBase(audioService)
    {
        [Command("help", RunMode = RunMode.Async, Summary = "Shows the help menu")]
        public async Task HelpAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("📜 **Help & Commands**")
                .WithDescription($"Use `{Prefix}` as the command prefix for all commands.")
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp()

                // Music Commands Section
                .AddField("🎵 **Music Commands**",
                    $"`{Prefix}join` - Joins your current voice channel\n" +
                    $"`{Prefix}leave` - Leaves the voice channel\n" +
                    $"`{Prefix}play <song/playlist>` - Plays a song or playlist from YouTube\n" +
                    $"`{Prefix}queue` - Shows the current song queue\n" +
                    $"`{Prefix}remove <index>` - Removes a song from the queue by index\n" +
                    $"`{Prefix}remove-all` - Removes all songs from the queue\n" +
                    $"`{Prefix}repeat` - Toggles repeat mode on/off\n" +
                    $"`{Prefix}skip` - Skips the current song\n" +
                    $"`{Prefix}volume <level>` - Sets the volume (0-100%)")

                // Filter Commands Section
                .AddField("🎛️ **Filter Commands**",
                    $"`{Prefix}show-filters` - Shows the current filter settings\n" +
                    $"`{Prefix}karaoke` - Applies the karaoke filter\n" +
                    $"`{Prefix}reverb` - Applies the reverb filter\n" +
                    $"`{Prefix}screw-it` - Applies the Screw-It (Houston Mode) filter\n" +
                    $"`{Prefix}timescale <speed> <pitch> <rate>` - Applies the timescale filter\n" +
                    $"`{Prefix}vibrato <frequency> <depth>` - Applies the vibrato filter")

                // Equalizer Commands Section
                .AddField("🎚️ **Equalizer Commands**",
                    $"`{Prefix}show-eq` - Displays the current equalizer settings\n" +
                    $"`{Prefix}set-eq <band>:<gain>` - Adjusts the EQ band gain\n" +
                    $"`{Prefix}reset-eq` - Resets all EQ settings to default")

                // Utility Commands Section
                .AddField("⚙️ **Utility Commands**",
                    $"`{Prefix}help` - Displays this help message\n" +
                    $"`{Prefix}player-counts` - Retrieves player count for Dark and Darker");

            await ReplyAsync(embed: embed.Build());
        }
    }
}