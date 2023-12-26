using FluentAssertions;
using JsonParser.Lexer;
using JsonParser.StateMachine;

namespace JsonParser.Tests.Lexer;

public class LiteralNameMatcherTests
{
    [Theory]
    [ClassData(typeof(LiteralNameMatcherTestData))]
    public void ShouldHaveCorrectResult(string stringToMatch, string stringToTest, StateMachineResult expectedResult)
    {
        // Arrange
        var sut = new LiteralNameMatcher(stringToMatch);

        // Act
        foreach (var character in stringToTest)
        {
            sut.Step(character);
        }

        // Assert
        sut.Result.Should().Be(expectedResult);
    }

    public class LiteralNameMatcherTestData : TheoryData<string, string, StateMachineResult>
    {
        public LiteralNameMatcherTestData()
        {
            Add("test", "", StateMachineResult.Processing);
            Add("test", "t", StateMachineResult.Processing);
            Add("test", "te", StateMachineResult.Processing);
            Add("test", "tes", StateMachineResult.Processing);
            Add("test", "test", StateMachineResult.Accepted);
            Add("test", "test1", StateMachineResult.Rejected);
            Add("test", "test12", StateMachineResult.Rejected);
            Add("test", "test123", StateMachineResult.Rejected);
            Add("test", "a", StateMachineResult.Rejected);
            Add("test", "ab", StateMachineResult.Rejected);
            Add("test", "abc", StateMachineResult.Rejected);
            Add("test", "abcd", StateMachineResult.Rejected);
            Add("test", "abcd1", StateMachineResult.Rejected);
            Add("test", "abcd12", StateMachineResult.Rejected);
            Add("test", "abcd123", StateMachineResult.Rejected);
            Add("test", "abcdtest123", StateMachineResult.Rejected);
            Add("test", "abcd123test", StateMachineResult.Rejected);
        }
    }
}
