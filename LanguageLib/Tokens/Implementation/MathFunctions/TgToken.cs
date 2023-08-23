using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation.MathFunctions
{
    public class TgToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Tg;
        public string Value { get; set; } = "tg";
        public string Regex { get; set; } = @"^tg$";
        public int Position { get; set; } = -1;

        public TgToken()
        {
        }

        public TgToken(string value, int position)
        {
            Value = value;
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);

        public IToken GetTokenObject(string value = "tg", int position = 0)
        {
            return new TgToken(value, position);
        }
    }
}
