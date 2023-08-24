using System.Linq;
using LanguageLib.Analyzers.Interfaces;
using LanguageLib.AST.Interfaces;
using LanguageLib.Errors;
using LanguageLib.Errors.Interfaces;
using LanguageLib.Tokens.Implementation;
using LanguageLib.Tokens.Implementation.NumberTokens;
using LanguageLib.Tokens.Implementation.Other;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Analyzers.Implementation
{
    public class SyntacticalAnalyzer : IAnalyzer, ISyntacticalAnalyzer
    {
        public List<IError> Errors { get; set; }
        public List<IToken> Tokens { get; set; }
        public IAST AST { get; set; }

        public int ErrorsCount
        {
            get => Errors.Count;
        }

        public SyntacticalAnalyzer(List<IToken> tokens)
        {
            Tokens = tokens;
            Errors = new List<IError>();
        }

        public void Analyze()
        {
            if (Tokens.Count == 0)
            {
                Errors.Add(new SyntacticalError("Пустой ввод", 0));
            }

            if (Tokens[0] is not StartToken)
            {
                Errors.Add(new SyntacticalError("Первое слово не 'Start'", 0));
            }

            if (Tokens[Tokens.Count - 1] is not StopToken)
            {
                Errors.Add(new SyntacticalError("Последнее слово не 'Stop'", Tokens.Count - 1));
            }

            #region first, second, third, fourth

            var token = Tokens[1];
            if (token is not FirstToken || token is not SecondToken || token is not ThirdToken ||
                token is not FourthToken)
            {
                Errors.Add(new SyntacticalError("Второе слово не 'First', 'Second', 'Third', 'Fourth'", 1));
            }
            else
            {
                if (token is FirstToken)
                {
                    int currentTokenIndex = 2;
                    var currentToken = Tokens[currentTokenIndex];

                    while (currentToken is not )
                    {
                        
                    }
                }
                if(token is SecondToken) { }
                if(token is ThirdToken) { }
                if(token is FourthToken) { }
            }


            #endregion

        }

        public void MakeAST()
        {
            throw new NotImplementedException();
        }
    }
}
