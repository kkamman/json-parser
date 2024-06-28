using JsonParser.StateMachine;

namespace JsonParser.Lexer;

public enum JsonNumberMatcherState
{
    Start,
    NoMatch,
    Minus,
    Zero,
    IntegerDigit,
    DecimalPoint,
    FractionalDigit,
    ExponentDigit,
    ExponentE,
    ExponentSign
}

public class JsonNumberMatcher : IStateMachine<char, JsonNumberMatcherState>
{
    public StateMachineResult Result => State switch
    {
        JsonNumberMatcherState.Zero
        or JsonNumberMatcherState.IntegerDigit
        or JsonNumberMatcherState.FractionalDigit
        or JsonNumberMatcherState.ExponentDigit
            => StateMachineResult.Accepted,

        JsonNumberMatcherState.NoMatch
            => StateMachineResult.Rejected,

        _ => StateMachineResult.Processing,
    };

    public JsonNumberMatcherState State { get; private set; } = JsonNumberMatcherState.Start;

    public void Step(char character)
    {
        State = State switch
        {
            JsonNumberMatcherState.Start when character is '-'
                => JsonNumberMatcherState.Minus,

            JsonNumberMatcherState.Start or JsonNumberMatcherState.Minus when character is '0'
                => JsonNumberMatcherState.Zero,

            JsonNumberMatcherState.Start or JsonNumberMatcherState.Minus when character is >= '1' and <= '9'
                => JsonNumberMatcherState.IntegerDigit,

            JsonNumberMatcherState.IntegerDigit when character is >= '0' and <= '9'
                => JsonNumberMatcherState.IntegerDigit,

            JsonNumberMatcherState.IntegerDigit or JsonNumberMatcherState.Zero when character is '.'
                => JsonNumberMatcherState.DecimalPoint,

            JsonNumberMatcherState.DecimalPoint or JsonNumberMatcherState.FractionalDigit when character is >= '0' and <= '9'
                => JsonNumberMatcherState.FractionalDigit,

            JsonNumberMatcherState.Zero or JsonNumberMatcherState.FractionalDigit when character is 'e' or 'E'
                => JsonNumberMatcherState.ExponentE,

            JsonNumberMatcherState.ExponentE when character is >= '0' and <= '9'
                => JsonNumberMatcherState.ExponentDigit,

            JsonNumberMatcherState.ExponentE when character is '-' or '+'
                => JsonNumberMatcherState.ExponentSign,

            JsonNumberMatcherState.ExponentSign when character is >= '0' and <= '9'
                => JsonNumberMatcherState.ExponentDigit,

            _ => JsonNumberMatcherState.NoMatch
        };
    }

    public void Reset()
    {
        State = JsonNumberMatcherState.Start;
    }
}
