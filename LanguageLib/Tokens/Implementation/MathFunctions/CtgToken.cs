using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation.MathFunctions
{
    public class CtgToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Ctg;
        public string Value { get; set; } = "ctg";
        public string Regex { get; set; } = @"^ctg$";
        public int Position { get; set; } = -1;

        public CtgToken()
        {
        }

        private CtgToken(string value, int position)
        {
            Position = position;
            Value = value;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);
        
        public IToken GetTokenObject(string value = "ctg", int position = 0)
        {
            return new CtgToken(value, position);
        }
    }
}
