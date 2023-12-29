using JsonParser.StateMachine;

namespace JsonParser.Lexer;

public enum JsonTokenMatcherState
{
    Start,
    NoMatch,
    True,
    Null,
    False,
    BeginObject,
    BeginArray,
    EndObject,
    EndArray,
    NameSeparator,
    ValueSeparator,
    String,
    Number
}

public class JsonTokenMatcher : IStateMachine<char, JsonTokenMatcherState>
{
    private readonly IReadOnlyDictionary<JsonTokenMatcherState, IStateMachine<char>> _submachinesByState =
        new Dictionary<JsonTokenMatcherState, IStateMachine<char>>
        {
            { JsonTokenMatcherState.True, new LiteralNameMatcher("true") },
            { JsonTokenMatcherState.Null, new LiteralNameMatcher("null") },
            { JsonTokenMatcherState.False, new LiteralNameMatcher("false") }
        };

    private IStateMachine<char>? _currentSubmachine = null;

    public StateMachineResult Result => State switch
    {
        JsonTokenMatcherState.Start => StateMachineResult.Processing,
        JsonTokenMatcherState.NoMatch => StateMachineResult.Rejected,
        JsonTokenMatcherState.BeginObject => StateMachineResult.Accepted,
        JsonTokenMatcherState.BeginArray => StateMachineResult.Accepted,
        JsonTokenMatcherState.EndObject => StateMachineResult.Accepted,
        JsonTokenMatcherState.EndArray => StateMachineResult.Accepted,
        JsonTokenMatcherState.NameSeparator => StateMachineResult.Accepted,
        JsonTokenMatcherState.ValueSeparator => StateMachineResult.Accepted,
        _ when _currentSubmachine is not null => _currentSubmachine.Result,
        _ => throw new NotImplementedException(),
    };

    public JsonTokenMatcherState State { get; private set; } = JsonTokenMatcherState.Start;

    public void Step(char character)
    {
        if (State != JsonTokenMatcherState.String && char.IsWhiteSpace(character))
        {
            return;
        }

        if (State == JsonTokenMatcherState.Start)
        {
            MoveToState(character switch
            {
                't' => JsonTokenMatcherState.True,
                'n' => JsonTokenMatcherState.Null,
                'f' => JsonTokenMatcherState.False,
                '{' => JsonTokenMatcherState.BeginObject,
                '[' => JsonTokenMatcherState.BeginArray,
                '}' => JsonTokenMatcherState.EndObject,
                ']' => JsonTokenMatcherState.EndArray,
                ':' => JsonTokenMatcherState.NameSeparator,
                ',' => JsonTokenMatcherState.ValueSeparator,
                '-' or (>= '1' and <= '9') => JsonTokenMatcherState.Number,
                '\"' => JsonTokenMatcherState.String,
                _ => JsonTokenMatcherState.NoMatch
            });
        }

        _currentSubmachine?.Step(character);
    }

    private void MoveToState(JsonTokenMatcherState nextState)
    {
        State = nextState;
        _currentSubmachine = _submachinesByState
            .TryGetValue(nextState, out var submachine)
            ? submachine : null;
    }

    public void Reset()
    {
        State = JsonTokenMatcherState.Start;
        foreach (var matcher in _submachinesByState.Values)
        {
            matcher.Reset();
        }
    }
}
