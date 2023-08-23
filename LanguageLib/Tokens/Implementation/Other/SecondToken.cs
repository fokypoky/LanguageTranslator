using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageLib.Tokens.Implementation.Other
{
    public class SecondToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Second;
        public string Value { get; set; } = "second";
        public string Regex { get; set; } = @"^second$";
        public int Position { get; set; } = -1;

        public SecondToken()
        {
        }

        private SecondToken(string value, int position)
        {
            Value = value;
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);

        public IToken GetTokenObject(string value = "third", int position = 0)
        {
            return new SecondToken(value, position);
        }
    }
}
