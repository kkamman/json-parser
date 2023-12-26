using JsonParser.StateMachine;

namespace JsonParser.Lexer;

public class LiteralNameMatcher : IStateMachine<int, char, LiteralNameMatcher>
{
    private readonly string _literalName;

    public int State { get; private set; }

    public StateMachineResult Result => State switch
    {
        -1 => StateMachineResult.Rejected,
        _ => State < _literalName.Length ?
            StateMachineResult.Processing :
            StateMachineResult.Accepted
    };

    public LiteralNameMatcher(string literalName)
    {
        _literalName = literalName;
    }

    public LiteralNameMatcher Step(char character)
    {
        if (Result == StateMachineResult.Processing
            && character == _literalName[State])
        {
            State++;
        }
        else
        {
            State = -1;
        }
        return this;
    }

    public LiteralNameMatcher Reset()
    {
        State = 0;
        return this;
    }
}
