using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.AST.Interfaces;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Analyzers.Interfaces
{
    public interface ISyntacticalAnalyzer
    {
        public IAST AST { get; set; }
        void MakeAST();
        List<IToken> Tokens { get; set; }
    }
}
