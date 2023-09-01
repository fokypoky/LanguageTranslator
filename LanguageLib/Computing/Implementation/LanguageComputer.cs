using LanguageLib.AST.Interfaces;
using LanguageLib.Computing.Interfaces;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Computing.Implementation
{
    public class LanguageComputer : ILanguageComputer
    {
        public List<IVariableToken> ResultList { get; set; }
        public IAST AST { get; set; }
        
        public LanguageComputer(IAST ast)
        {
            AST = ast;
            ResultList = new List<IVariableToken>();
        }

        public void Compute()
        {
            throw new NotImplementedException();
        }
    }
}
