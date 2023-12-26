namespace JsonParser.StateMachine;

public interface IStateMachine<TState, TStep, TSelf>
    where TSelf : IStateMachine<TState, TStep, TSelf>
{
    public TState State { get; }
    public StateMachineResult Result { get; }
    public TSelf Step(TStep value);
    public TSelf Reset();
}
