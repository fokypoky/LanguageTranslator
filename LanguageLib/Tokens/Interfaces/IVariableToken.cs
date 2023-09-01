using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageLib.Tokens.Interfaces
{
    public interface IVariableToken : IToken
    {
        string Name { get; set; }
    }
}
