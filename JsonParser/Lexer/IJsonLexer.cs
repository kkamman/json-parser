namespace JsonParser.Lexer;

public interface IJsonLexer
{
    IEnumerable<string> Tokenize(string json);
}
