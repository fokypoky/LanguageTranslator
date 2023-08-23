using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation
{
    public class StopToken : IToken
    {
        public TokenType Type { get; set; } = TokenType.Stop;
        public string Value { get; set; } = "stop";
        public string Regex { get; set; } = @"^stop$";
        public int Position { get; set; } = -1;

        public StopToken()
        {
        }

        private StopToken(string value, int position)
        {
            Value = value;
            Position = position;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);
        
        public IToken GetTokenObject(string value = "stop", int position = 0)
        {
            return new StopToken(value, position);
        }
    }
}
