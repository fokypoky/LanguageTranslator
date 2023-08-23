using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation.MathFunctions
{
    public class CosToken : IToken
    {
        public string Value { get; set; } = "cos";
        public TokenType Type { get; set; } = TokenType.Cos;
        public string Regex { get; set; } = @"^cos$";
        public int Position { get; set; } = -1;

        public CosToken()
        {
        }

        private CosToken(string value, int position)
        {
            Position = position;
            Value = value;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);

        public IToken GetTokenObject(string value = "cos", int position = 0)
        {
            return new CosToken(value, position);
        }
    }
}
