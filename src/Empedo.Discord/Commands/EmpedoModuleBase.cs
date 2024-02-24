using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Empedo.Discord.Services;
using Microsoft.Extensions.Logging;

namespace Empedo.Discord.Commands
{
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class EmpedoModuleBase : BaseCommandModule
    {
        public ILogger Logger { get; set; }
        public ITempusEmbedService TempusEmbedService { get; set; }
    }
}