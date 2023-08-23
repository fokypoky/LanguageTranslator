using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageLib.Tokens.Implementation.Other
{
    public class ThirdToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Third;
        public string Value { get; set; } = "third";
        public string Regex { get; set; } = @"^third$";
        public int Position { get; set; } = -1;

        public ThirdToken()
        {
        }

        private ThirdToken(string value, int position)
        {
            Value = value;
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);

        public IToken GetTokenObject(string value = "third", int position = 0)
        {
            return new ThirdToken(value, position);
        }
    }
}
