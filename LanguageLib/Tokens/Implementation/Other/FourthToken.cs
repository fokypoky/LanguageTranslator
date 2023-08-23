using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageLib.Tokens.Implementation.Other
{
    public class FourthToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Fourth;
        public string Value { get; set; } = "fourth";
        public string Regex { get; set; } = @"^fourth$";
        public int Position { get; set; } = -1;

        public FourthToken()
        {
        }

        private FourthToken(string value, int position)
        {
            Value = value;
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);

        public IToken GetTokenObject(string value = "first", int position = 0)
        {
            return new FourthToken(value, position);
        }
    }
}
