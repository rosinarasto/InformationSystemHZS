using InformationSystemHZS.Services.Commands.Interfaces;

namespace InformationSystemHZS.Services.Commands;

public class AddAnswer(string[] args) : ICommandAnswer
{
    private string[] args = args;
    
    public IEnumerable<string> ProcessAnswer()
    {
        yield return $"[processed]: {args[4]} was added to unit {args[3]}.";
    }
}
