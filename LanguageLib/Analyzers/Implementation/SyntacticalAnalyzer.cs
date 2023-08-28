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

            // TODO: проверить второе слово языка

            if (!TokenIsLinkWord(Tokens[1]))
            {
                Errors.Add(new SyntacticalError("Второе слово не First, Second, Third, Fourth", 1));
                return;
            }

            // поиск токенов звена

            int linkTokensEndIndex = -1;

            for (int i = 1; i < Tokens.Count; i++)
            {
                var token = Tokens[i];

                if ((i + 1) < Tokens.Count - 1 && token is IntegerToken && Tokens[i + 1] is ColonToken)
                {
                    linkTokensEndIndex = i - 1;
                    break;
                }

                if (token is VariableToken)
                {
                    linkTokensEndIndex = i - 1;
                    break;
                }
            }

            if (linkTokensEndIndex == -1)
            {
                Errors.Add(new SyntacticalError("Токены не найдены", 1));
                return;
            }

            var linkTokens = new List<IToken>();

            for (int i = 1; i <= linkTokensEndIndex; i++)
            {
                linkTokens.Add(Tokens[i]);
            }
            // анализ токенов

            bool isLinkTokensSuccess = AnalyzeLinks(ref linkTokens, 1);
            if (!isLinkTokensSuccess)
            {
                return;
            }
            #endregion

            #region Operators

            // TODO: найти операторы и передать в функцию
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

            // получаю индексы токенов First, Second, Third, Fourth
            var tokensIndexes = new List<int>();
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token is FirstToken || token is SecondToken || token is ThirdToken || token is FourthToken)
                {
                    tokensIndexes.Add(i);
                }
            }

            // проверка

            // после First, Second, Third, Fourth может ничего не быть

            if (TokenIsLinkWord(tokens[tokens.Count - 1]))
            {
                Errors.Add(
                    new SyntacticalError($"После '{tokens[tokens.Count - 1].Value}' ничего нет",
                        Tokens.IndexOf(tokens[tokens.Count - 1]))
                );
                return false;
            }

            // последнее слово может быть запятой

            if (tokens[tokens.Count - 1] is CommaToken)
            {
                Errors.Add(new SyntacticalError("Ожидается 'First', 'Second', 'Third', Fourth",
                    tokens[tokens.Count - 1].Position));
                return false;
            }

            // после запятой может быть не First, Second, Third, Fourth

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is CommaToken)
                {
                    var token = tokens[i + 1];
                    if (!TokenIsLinkWord(token))
                    {
                        Errors.Add(
                            new SyntacticalError($"После запятой ожидалось 'First', 'Second', 'Third', 'Fourth'",
                                token.Position + 1)
                        );
                    }
                }
            }

            // проверка токена(что идет после него)
            foreach (int tokenIndex in tokensIndexes)
            {
                var token = tokens[tokenIndex];

                // после слова может ничего не быть(последнее слово)
                if (tokenIndex == tokens.Count - 1)
                {
                    Errors.Add(new SyntacticalError($"После слова '{token.Value}' ничего нет", token.Position));
                    return false;
                }

                // после слова может ничего не быть (запятая)
                if (tokens[tokenIndex + 1] is CommaToken)
                {
                    Errors.Add(
                        new SyntacticalError($"После слова {token.Value} стоит запятая, но нет вещественных/целых чисел", 
                            token.Position + 1)
                    );
                    return false;
                }

                if (token is FirstToken)
                {
                    // "First" Вещественное
                    if ((tokenIndex + 1) <= tokens.Count - 1 && tokens[tokenIndex + 1] is not DecimalToken)
                    {
                        Errors.Add(new SyntacticalError("Ожидалось вещественное число", tokenIndex + 1));
                        return false;
                    }

                    // проверка: третье слово - запятая
                    if ((tokenIndex + 2) < tokens.Count - 2 && tokens[tokenIndex + 2] is not CommaToken)
                    {
                        Errors.Add(new SyntacticalError("Ожидалось запятая", tokenIndex + 2));
                        return false;
                    }
                }
                
                else if (token is SecondToken)
                {
                    // определение конца токена
                    int secondTokenEndIndex = -1;

                    for (int i = tokenIndex; i < tokens.Count - 1; i++)
                    {
                        if (tokens[i] is CommaToken)
                        {
                            secondTokenEndIndex = i;
                            break;
                        }

                        if (i == tokens.Count - 1)
                        {
                            secondTokenEndIndex = i;
                            break;
                        }
                    }

                    // определение токенов после слова Second

                    var secondWordSubTokens = new List<IToken>();
                    
                    for (int i = tokenIndex; i < secondTokenEndIndex; i++)
                    {
                        secondWordSubTokens.Add(tokens[i]);
                    }


                    // проверка вещественных чисел россыпью
                    for (int i = 1; i < secondWordSubTokens.Count; i++)
                    {
                        if (secondWordSubTokens[i] is not DecimalToken)
                        {
                            Errors.Add(new SyntacticalError("Ожидалось вещественное число", secondWordSubTokens[i].Position));
                            return false;
                        }
                    }
                    
                }

                else if (token is ThirdToken) { }
                else if (token is FourthToken) { }
            }

            return true;
        }

        private bool AnalyzeOperators(List<IToken> tokens)
        {
            return true;
        }

        private bool TokenIsLinkWord(IToken token) =>
            token is FirstToken || token is SecondToken || token is ThirdToken || token is FourthToken;
    }
}
