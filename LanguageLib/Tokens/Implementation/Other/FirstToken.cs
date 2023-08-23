using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation.Other
{
    public class FirstToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.First;
        public string Value { get; set; } = "first";
        public string Regex { get; set; } = @"^first$";
        public int Position { get; set; } = -1;

        public FirstToken()
        {
        }

        private FirstToken(string value, int position)
        {
            Value = value;
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);

        public IToken GetTokenObject(string value = "first", int position = 0)
        {
            return new FirstToken(value, position);
        }
    }
}
