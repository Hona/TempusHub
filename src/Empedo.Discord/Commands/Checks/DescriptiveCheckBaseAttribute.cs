using DSharpPlus.CommandsNext.Attributes;

namespace Empedo.Discord.Commands.Checks
{
    public abstract class DescriptiveCheckBaseAttribute : CheckBaseAttribute
    {
        public string FailureResponse { get; set; }
    }
}