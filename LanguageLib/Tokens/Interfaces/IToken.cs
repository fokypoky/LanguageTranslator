using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation;
using LanguageLib.Tokens.Implementation.Enums;

namespace LanguageLib.Tokens.Interfaces
{
    public interface IToken
    {
        TokenType Type { get; set; }
        string Value { get; set; }
        string Regex { get; set; }
        int Position { get; set; }
        bool IsMatch(string word);
        IToken GetTokenObject(string value, int position);
    }
}
