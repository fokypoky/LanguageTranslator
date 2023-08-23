using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation
{
    public class StartToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Start;
        public string Value { get; set; } = "start";
        public string Regex { get; set; } = @"^start$";
        public int Position { get; set; } = -1;

        public StartToken()
        {
        }

        private StartToken(string value, int position)
        {
            Position = position;
            Value = value;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);
        
        public IToken GetTokenObject(string value = "start", int position = 0)
        {
            return new StartToken(value, position);
        }
    }
}
