using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation.TextFormat
{
    public class TabulationToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Tabulation;
        public string Value { get; set; } = "\t";
        public string Regex { get; set; } = @"^\\t$";
        public int Position { get; set; } = -1;

        public TabulationToken()
        {
        }

        private TabulationToken(string value, int position)
        {
            Value = value;
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);
        
        public IToken GetTokenObject(string value = "\t", int position = 0)
        {
            return new TabulationToken(value, position);
        }
    }
}
