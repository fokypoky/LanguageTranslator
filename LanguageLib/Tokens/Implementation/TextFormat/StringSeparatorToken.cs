using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation.TextFormat
{
    public class StringSeparatorToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.StringSeparator;
        public string Value { get; set; } = "\n";
        public string Regex { get; set; } = @"^\\n$";
        public int Position { get; set; } = -1;

        public StringSeparatorToken()
        {
        }

        private StringSeparatorToken(string value, int position)
        {
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);
        
        public IToken GetTokenObject(string value = "\n", int position = 0)
        {
            return new StringSeparatorToken(value, position);
        }
    }
}
