using JsonParser.StateMachine;

namespace JsonParser.Lexer;

public class JsonLexer : IJsonLexer
{
    public IEnumerable<string> Tokenize(string json)
    {
        var jsonTokenMatcher = new JsonTokenMatcher();
        var lexeme = string.Empty;
        foreach (var character in json)
        {
            lexeme += character;
            jsonTokenMatcher.Step(character);
            if (jsonTokenMatcher.Result == StateMachineResult.Accepted)
            {
                yield return lexeme;
            }
            else if (jsonTokenMatcher.Result == StateMachineResult.Rejected)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
