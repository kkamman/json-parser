using FluentAssertions;
using JsonParser.Lexer;

namespace JsonParser.Tests.Lexer;

public class JsonLexerTests
{
    [Theory]
    [ClassData(typeof(JsonLexerTestData))]
    public void ShouldMatchJsonTokens(string json, string[] tokens)
    {
        // Arrange
        var sut = new JsonLexer();

        // Act
        var result = sut.Tokenize(json);

        // Assert
        result.Should().Equal(tokens);
    }

    public class JsonLexerTestData : TheoryData<string, string[]>
    {
        public JsonLexerTestData()
        {
            Add("true \"test\" { } [ ]  false    null   0 0.123 123.0123", ["true", "\"test\"", "{", "}", "[", "]", "false", "null", "0", "0.123", "123.0123"]);
        }
    }
}
