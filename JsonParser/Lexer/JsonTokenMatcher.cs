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
            { JsonTokenMatcherState.False, new LiteralNameMatcher("false") },
            { JsonTokenMatcherState.String, new JsonStringMatcher() },
            { JsonTokenMatcherState.Number, new JsonNumberMatcher() },
        };

    private IStateMachine<char>? _submachine = null;

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
        _ when _submachine is not null => _submachine.Result,
        _ => throw new NotImplementedException(),
    };

    public JsonTokenMatcherState State { get; private set; } = JsonTokenMatcherState.Start;

    public void Step(char character)
    {
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
                '-' or (>= '0' and <= '9') => JsonTokenMatcherState.Number,
                '\"' => JsonTokenMatcherState.String,
                _ => JsonTokenMatcherState.NoMatch
            });
        }
        else if (_submachine is null)
        {
            State = JsonTokenMatcherState.NoMatch;
        }

        _submachine?.Step(character);
    }

    private void MoveToState(JsonTokenMatcherState nextState)
    {
        State = nextState;
        _submachine ??= _submachinesByState.GetValueOrDefault(nextState);
    }

    public void Reset()
    {
        State = JsonTokenMatcherState.Start;
        _submachine?.Reset();
        _submachine = null;
    }
}
