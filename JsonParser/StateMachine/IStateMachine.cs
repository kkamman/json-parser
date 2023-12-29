namespace JsonParser.StateMachine;

public interface IStateMachine<TStep>
{
    public StateMachineResult Result { get; }
    public void Step(TStep value);
    public void Reset();
}

public interface IStateMachine<TStep, TState> : IStateMachine<TStep>
{
    public TState State { get; }
}
