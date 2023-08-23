using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation.NumberTokens
{
    public class IntegerToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Integer;
        public string Value { get; set; } = "";
        public string Regex { get; set; } = @"^[0-7]+$";
        public int Position { get; set; } = -1;

        public IntegerToken()
        {
        }

        private IntegerToken(string value, int position)
        {
            Value = value;
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);

        public IToken GetTokenObject(string value, int position = 0)
        {
            return new IntegerToken(value, position);
        }
    }
}
