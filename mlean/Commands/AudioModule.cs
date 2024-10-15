using Discord.Commands;
using ContextType = Discord.Interactions.ContextType;

namespace mlean.Commands;

[Discord.Interactions.RequireContext(ContextType.Guild)]
public class AudioModule : ModuleBase<SocketCommandContext>
{
}