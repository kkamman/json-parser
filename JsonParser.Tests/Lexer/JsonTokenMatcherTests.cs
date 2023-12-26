using FluentAssertions;
using JsonParser.Lexer;
using JsonParser.StateMachine;

namespace JsonParser.Tests.Lexer;

public class JsonTokenMatcherTests
{
    [Theory]
    [ClassData(typeof(JsonTokenMatcherTestData))]
    public void ShouldMatchLiteralNames(string literalName, StateMachineResult expectedResult)
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
    }

    public class JsonTokenMatcherTestData : TheoryData<string, StateMachineResult>
    {
        public JsonTokenMatcherTestData()
        {
            Add("true", StateMachineResult.Accepted);
            Add("null", StateMachineResult.Accepted);
            Add("false", StateMachineResult.Accepted);
        }
    }
}
