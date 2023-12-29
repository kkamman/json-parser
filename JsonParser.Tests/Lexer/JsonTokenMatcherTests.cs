using FluentAssertions;
using JsonParser.Lexer;
using JsonParser.StateMachine;

namespace JsonParser.Tests.Lexer;

public class JsonTokenMatcherTests
{
    [Theory]
    [ClassData(typeof(JsonTokenMatcherTestData))]
    public void ShouldMatchJsonTokens(
        string literalName,
        StateMachineResult expectedResult,
        JsonTokenMatcherState expectedState)
    {
        // Arrange
        var sut = new JsonTokenMatcher();

        // Act
        foreach (var character in literalName)
        {
            sut.Step(character);
        }

        // Assert
        sut.Result.Should().Be(expectedResult);
        sut.State.Should().Be(expectedState);
    }

    public class JsonTokenMatcherTestData : TheoryData<string, StateMachineResult, JsonTokenMatcherState>
    {
        public JsonTokenMatcherTestData()
        {
            Add("true", StateMachineResult.Accepted, JsonTokenMatcherState.True);
            Add("null", StateMachineResult.Accepted, JsonTokenMatcherState.Null);
            Add("false", StateMachineResult.Accepted, JsonTokenMatcherState.False);
            Add("{", StateMachineResult.Accepted, JsonTokenMatcherState.BeginObject);
            Add("[", StateMachineResult.Accepted, JsonTokenMatcherState.BeginArray);
            Add("}", StateMachineResult.Accepted, JsonTokenMatcherState.EndObject);
            Add("]", StateMachineResult.Accepted, JsonTokenMatcherState.EndArray);
            Add(":", StateMachineResult.Accepted, JsonTokenMatcherState.NameSeparator);
            Add(",", StateMachineResult.Accepted, JsonTokenMatcherState.ValueSeparator);
        }
    }
}
