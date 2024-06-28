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
            if (lexeme == string.Empty && char.IsWhiteSpace(character))
            {
                continue;
            }

            var resultBeforeStep = jsonTokenMatcher.Result;

            if (resultBeforeStep is StateMachineResult.Rejected)
            {
                throw new InvalidOperationException();
            }

            jsonTokenMatcher.Step(character);

            if (resultBeforeStep is StateMachineResult.Accepted
                && jsonTokenMatcher.Result is StateMachineResult.Rejected)
            {
                yield return lexeme;
                lexeme = string.Empty;
                jsonTokenMatcher.Reset();
            }

            if (!char.IsWhiteSpace(character))
            {
                lexeme += character;
            }
        }

        if (lexeme != string.Empty)
        {
            yield return jsonTokenMatcher.Result is StateMachineResult.Accepted
                ? lexeme
                : throw new InvalidOperationException();
        }
    }
}
