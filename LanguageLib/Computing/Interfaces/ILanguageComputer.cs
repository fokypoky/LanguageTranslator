using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.AST.Interfaces;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Computing.Interfaces
{
    public interface ILanguageComputer
    {
        List<IVariableToken> ResultList { get; set; } 
        IAST AST { get; set; }
        void Compute();
    }
}
