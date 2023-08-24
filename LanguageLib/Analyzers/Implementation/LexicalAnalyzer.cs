using System.Text;
using LanguageLib.Analyzers.Interfaces;
using LanguageLib.Errors.Implementation;
using LanguageLib.Errors.Interfaces;
using LanguageLib.Tokens.Implementation;
using LanguageLib.Tokens.Implementation.MathFunctions;
using LanguageLib.Tokens.Implementation.MathOperations;
using LanguageLib.Tokens.Implementation.NumberTokens;
using LanguageLib.Tokens.Implementation.Other;
using LanguageLib.Tokens.Implementation.TextFormat;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Analyzers.Implementation
{
    public class LexicalAnalyzer : IAnalyzer, ILexicalAnalyzer
    {
        public List<IError> Errors { get; set; }
        public List<IToken> Tokens { get; set; }

        public int ErrorsCount
        {
            get => Errors.Count;
        }

        private List<IToken> _testTokens;

        private string _text;

        public LexicalAnalyzer(string text)
        {
            _text = text;

            Tokens = new List<IToken>();
            Errors = new List<IError>();
            
            // first, second, third, fourth - добавить в начало, чтобы не определялись как переменные
            _testTokens = new List<IToken>()
            {
                new StartToken(), new StopToken(),
                new FirstToken(), new SecondToken(),
                new ThirdToken(), new FourthToken(),
                new SinToken(), new CosToken(),
                new TgToken(), new CtgToken(),
                new IntegerToken(), new DecimalToken(),
                new VariableToken(), 
                new PlusToken(), new MinusToken(),
                new MultiplyToken(), new DivisionToken(),
                new ExpToken(),
                new AssignToken(), new ColonToken(),
                new TabulationToken(), new StringSeparatorToken(),
                new CommaToken()
            };

            PrepareText();
        }

        public void Analyze()
        {
            var rowsList = _text.Split("\r\n").ToList();
            rowsList.RemoveAll(s => s == "");

            int currentWordIndex = 0;

            for (int rowIndex = 0; rowIndex < rowsList.Count; rowIndex++)
            {
                var wordsList = rowsList[rowIndex].Split(" ").ToList();
                wordsList.RemoveAll(s => s == "" || s == " ");


                for (int wordIndex = 0; wordIndex < wordsList.Count; wordIndex++)
                {
                    string word = wordsList[wordIndex];

                    if (String.IsNullOrEmpty(word))
                    {
                        continue;
                    }

                    MakeToken(word, wordIndex);
                    currentWordIndex++;
                }
            }
        }

        private void PrepareText()
        {
            var cleanerStringBuilder = new StringBuilder(_text.ToLower());
            cleanerStringBuilder
                .Replace(",", " , ")
                .Replace(":", " : ")
                .Replace("+", " + ").Replace("-", " - ")
                .Replace("*", " * ").Replace("/", " / ")
                .Replace("^", " ^ ")
                .Replace("=", " = ")
                .Replace("sin", " sin ").Replace("cos", " cos ")
                .Replace("tg", " tg ").Replace("ctg", " ctg ");

            _text = cleanerStringBuilder.ToString();

        }

        private void MakeToken(string word, int position)
        {
            foreach (var token in _testTokens)
            {
                // first, second, third, fourth - определяются как переменные
                if (token.IsMatch(word))
                {
                    Tokens.Add(token.GetTokenObject(word, position));
                    return;
                }
            }

            Errors.Add(new LexicalError($"Слова {word} в языке не существует", position));
        }
    }
}
