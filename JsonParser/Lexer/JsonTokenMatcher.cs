using JsonParser.StateMachine;

namespace JsonParser.Lexer;

public enum JsonTokenMatcherState
{
    Start,
    True,
    Null,
    False,
}

public class JsonTokenMatcher : IStateMachine<JsonTokenMatcherState, char, JsonTokenMatcher>
{
    private readonly IReadOnlyDictionary<JsonTokenMatcherState, LiteralNameMatcher> _literalNameMatchersByState =
        new Dictionary<JsonTokenMatcherState, LiteralNameMatcher>
        {
            { JsonTokenMatcherState.True, new("true") },
            { JsonTokenMatcherState.Null, new("null") },
            { JsonTokenMatcherState.False, new("false") }
        };

    public StateMachineResult Result => State switch
    {
        JsonTokenMatcherState.Start => StateMachineResult.Processing,
        JsonTokenMatcherState.True => _literalNameMatchersByState[State].Result,
        JsonTokenMatcherState.Null => _literalNameMatchersByState[State].Result,
        JsonTokenMatcherState.False => _literalNameMatchersByState[State].Result,
        _ => throw new NotImplementedException(),
    };

    public JsonTokenMatcherState State { get; private set; } = JsonTokenMatcherState.Start;

    public JsonTokenMatcher Step(char character)
    {
        State = State switch
        {
            JsonTokenMatcherState.Start => character switch
            {
                't' or 'n' or 'f' => StepLiteralName(character),
                _ => throw new NotImplementedException(),
            },
            JsonTokenMatcherState.True => StepLiteralName(character),
            JsonTokenMatcherState.Null => StepLiteralName(character),
            JsonTokenMatcherState.False => StepLiteralName(character),
            _ => throw new NotImplementedException(),
        };
        return this;
    }

    private JsonTokenMatcherState StepLiteralName(char character)
    {
        var nextState = State switch
        {
            JsonTokenMatcherState.Start => character switch
            {
                't' => JsonTokenMatcherState.True,
                'n' => JsonTokenMatcherState.Null,
                'f' => JsonTokenMatcherState.False,
                _ => throw new InvalidOperationException()
            },
            _ => State
        };

        _literalNameMatchersByState[nextState].Step(character);

        return nextState;
    }

    public JsonTokenMatcher Reset()
    {
        State = JsonTokenMatcherState.Start;
        foreach (var matcher in _literalNameMatchersByState.Values)
        {
            matcher.Reset();
        }
        return this;
    }
}
