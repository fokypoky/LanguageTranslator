using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation.MathOperations
{
    public class MultiplyToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Multiply;
        public string Value { get; set; } = "*";
        public string Regex { get; set; } = @"^\*$";
        public int Position { get; set; } = -1;

        public MultiplyToken()
        {
        }

        private MultiplyToken(string value, int position)
        {
            Value = value;
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);
        
        public IToken GetTokenObject(string value = "*", int position = 0)
        {
            return new MultiplyToken(value, position);
        }
    }
}
