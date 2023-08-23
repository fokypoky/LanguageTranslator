using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation.MathFunctions
{
    public class SinToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Sin;
        public string Value { get; set; } = "sin";
        public string Regex { get; set; } = @"^sin$";
        public int Position { get; set; } = -1;

        public SinToken()
        {
        }

        private SinToken(string value, int position)
        {
            Position = position;
            Value = value;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);
        
        public IToken GetTokenObject(string value = "sin", int position = 0)
        {
            return new SinToken(value, position);
        }
    }
}
