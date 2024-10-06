using InformationSystemHZS.Services.Commands.Interfaces;

namespace InformationSystemHZS.Services.Commands
{
    internal class ReassingAnswer(string[] args, string callsign) : ICommandAnswer
    {
        private string[] args = args;
        private string callsign = callsign;

        public IEnumerable<string> ProcessAnswer()
        {
            yield return $"[processed]: The {args[1]} was reassigned under the new callsign {callsign}.";
        }
    }
}