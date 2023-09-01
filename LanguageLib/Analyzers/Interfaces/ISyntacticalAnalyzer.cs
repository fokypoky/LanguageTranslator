using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.AST.Interfaces
{
    public interface ISyntacticalAnalyzer
    {
        public IAST AST { get; set; }
        void MakeAST();
        List<IToken> Tokens { get; set; }
    }
}
