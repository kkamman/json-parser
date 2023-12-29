using JsonParser.StateMachine;

namespace JsonParser.Lexer;

public class LiteralNameMatcher : IStateMachine<char, int>
{
    private readonly string _literalName;
    private const int NO_MATCH_STATE = -1;

    public int State { get; private set; }

    public StateMachineResult Result => State switch
    {
        NO_MATCH_STATE => StateMachineResult.Rejected,
        _ => State < _literalName.Length ? StateMachineResult.Processing : StateMachineResult.Accepted
    };

    public LiteralNameMatcher(string literalName)
    {
        _literalName = literalName;
    }

    public void Step(char character)
    {
        if (Result == StateMachineResult.Processing
            && character == _literalName[State])
        {
            State++;
        }
        else
        {
            State = NO_MATCH_STATE;
        }
    }

    public void Reset()
    {
        State = 0;
    }
}
