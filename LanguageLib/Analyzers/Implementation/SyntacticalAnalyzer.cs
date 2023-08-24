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
            #region Start, Stop tokens && clear text checkout

            if (Tokens.Count == 0)
            {
                Errors.Add(new SyntacticalError("Пустой ввод", 0));
                return;
            }

            if (Tokens[0] is not StartToken)
            {
                Errors.Add(new SyntacticalError("Первое слово не 'Start'", 0));
                return;
            }

            if (Tokens[Tokens.Count - 1] is not StopToken)
            {
                Errors.Add(new SyntacticalError("Последнее слово не 'Stop'", Tokens.Count - 1));
                return;
            }

            #endregion

            #region first, second, third, fourth tokens

            var linkToken = Tokens[1];
            if (linkToken is not FirstToken && linkToken is not SecondToken && 
                linkToken is not ThirdToken && linkToken is not FourthToken)
            {
                Errors.Add(new SyntacticalError("Второе слово не 'First', 'Second', 'Third', 'Fourth'", 1));
            }
            else
            {
                #region operator tokens begin finding

                int operatorTokenIndex = -1;
                for (int i = 1; i < Tokens.Count; i++)
                {
                    var token = Tokens[i];

                    // оператор = </Метка ":"/> Переменная ...
                    if (token is VariableToken)
                    {
                        operatorTokenIndex = i;
                        break;
                    }
                }

                #endregion

                if (operatorTokenIndex != -1)
                {
                    // 2 - link word index
                    var linkTokensList = Tokens.GetRange(1, operatorTokenIndex - 2);
                    bool isListTokensSuccessful = AnalyzeLinks(ref linkTokensList, 1);
                    if (!isListTokensSuccessful)
                    {
                        return;
                    }
                }
                else
                {
                    Errors.Add(new SyntacticalError("Операторы не найдены", -1));
                    return;
                }
            }

            #endregion

            #region Operators

            bool isOperatorTokensSuccessful = AnalyzeOperators(null);
            if (!isOperatorTokensSuccessful)
            {
                return;
            }
            #endregion
        }

        public void MakeAST()
        {
            throw new NotImplementedException();
        }

        private bool AnalyzeLinks(ref List<IToken> tokens, int tokensPosition)
        {
            // Звено = "First" Вещ ! "Second" Вещ... Вещ ! "Third" Цел ! "Fourth" Цел... Цел
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                if (token is FirstToken)
                {
                    // i также может быть концом строки

                    if (i == tokens.Count - 1 && token is not DecimalToken)
                    {
                        Errors.Add(new SyntacticalError("Ожидалось вещественное число", i + tokensPosition));
                        break;
                    }

                    if(tokens[i + 1] is not DecimalToken || (i + 1) >= tokens.Count)
                    {
                        Errors.Add(new SyntacticalError("Ожидалось вещественное число", i + 1 + tokensPosition));
                        return false;
                    }

                    if(tokens[i + 2] is not CommaToken || (i + 2) >= tokens.Count)
                    {
                        Errors.Add(new SyntacticalError("Ожидалась запятая", i + 2 + tokensPosition));
                    }
                }
                else if (token is SecondToken)
                {
                    
                }
                else if(token is ThirdToken){}
                else if(token is FourthToken){}
            }

            return true;
        }

        private bool AnalyzeOperators(List<IToken> tokens)
        {
            return true;
        }
    }
}
