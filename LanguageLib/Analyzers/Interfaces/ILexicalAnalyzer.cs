using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Analyzers.Interfaces
{
    public interface ILexicalAnalyzer
    {
        List<IToken> Tokens { get; set; }
    }
}
