using JsonParser.StateMachine;

namespace JsonParser.Lexer;

public enum JsonStringMatcherState
{
    Start,
    NoMatch,
    Matched,
    Unescaped,
    Escaped,
    HexDigitStart,
    HexDigitOne,
    HexDigitTwo,
    HexDigitThree,
}

public class JsonStringMatcher : IStateMachine<char, JsonStringMatcherState>
{
    public StateMachineResult Result => State switch
    {
        JsonStringMatcherState.Matched => StateMachineResult.Accepted,
        JsonStringMatcherState.NoMatch => StateMachineResult.Rejected,
        _ => StateMachineResult.Processing,
    };

    public JsonStringMatcherState State { get; private set; } = JsonStringMatcherState.Start;

    public void Step(char character)
    {
        State = State switch
        {
            JsonStringMatcherState.Start when character is '"'
                => JsonStringMatcherState.Unescaped,

            JsonStringMatcherState.Unescaped when character is '"'
                => JsonStringMatcherState.Matched,

            JsonStringMatcherState.Unescaped when character is '\\'
                => JsonStringMatcherState.Escaped,

            JsonStringMatcherState.Unescaped when !char.IsControl(character)
                => JsonStringMatcherState.Unescaped,

            JsonStringMatcherState.Escaped when character is 'u'
                => JsonStringMatcherState.HexDigitStart,

            JsonStringMatcherState.Escaped when IsEscapableCharacter(character)
                => JsonStringMatcherState.Unescaped,

            JsonStringMatcherState.HexDigitStart when IsHexDigit(character)
                => JsonStringMatcherState.HexDigitOne,

            JsonStringMatcherState.HexDigitOne when IsHexDigit(character)
                => JsonStringMatcherState.HexDigitTwo,

            JsonStringMatcherState.HexDigitTwo when IsHexDigit(character)
                => JsonStringMatcherState.HexDigitThree,

            JsonStringMatcherState.HexDigitThree when IsHexDigit(character)
                => JsonStringMatcherState.Unescaped,

            _ => JsonStringMatcherState.NoMatch
        };
    }

    public void Reset()
    {
        State = JsonStringMatcherState.Start;
    }

    private static bool IsHexDigit(char character)
        => character is (>= '0' and <= '9') or (>= 'a' and <= 'f') or (>= 'A' and <= 'F');

    private static bool IsEscapableCharacter(char character)
        => character is '\\' or '/' or 'b' or 'f' or 'n' or 'r' or 't' or 'u';
}
