using System.Data;
using System.Text;
using LanguageLib.Analyzers.Implementation;
using LanguageLib.Tokens.Implementation.MathOperations;
using LanguageLib.Tokens.Implementation.NumberTokens;
using LanguageLib.Tokens.Interfaces;

namespace LanguageConsoleApp
{
    

    public class Test
    {
        public void Main()
        {
            string input = " 1.0 + - - sin -4.2 ^ sin cos tg 12.3 + 3.2 * 13.1 ^ 12.2 ^ - 3.0 + 7.2 * sin cos -30.1 + 4.1 ^ sin30.0 ";
            var lexer = new LexicalAnalyzer(input);
            lexer.Analyze();

            var tokens = lexer.Tokens;

            var parser = new SyntacticalAnalyzer(lexer.Tokens);

            // сбор токенов функций / серии функций

            #region Функции

            var funcTokensSeries = new List<FuncTokensSeria>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (parser.TokenIsMathFunctionToken(tokens[i]))
                {
                    var seria = new FuncTokensSeria();
                    seria.StartIndex = i;

                    // поиск конца серии токенов
                    for (int j = i + 1; j < tokens.Count; j++)
                    {
                        var token = tokens[j];

                        if (j == tokens.Count - 1)
                        {
                            seria.EndIndex = tokens.Count - 1;
                            i = tokens.Count - 1;
                            funcTokensSeries.Add(seria);
                            break;
                        }

                        if (!parser.TokenIsMathFunctionToken(token))
                        {
                            if (token is DecimalToken && (tokens[j + 1] is not ExpToken))
                            {
                                //funcTokensEndIndex = j;
                                seria.EndIndex = j;
                                i = j;
                                funcTokensSeries.Add(seria);
                                break;
                            }

                            if (token is PlusToken || token is MultiplyToken || token is DivisionToken)
                            {
                                //funcTokensEndIndex = j - 1;
                                seria.EndIndex = j - 1;
                                i = j - 1;
                                funcTokensSeries.Add(seria);
                                break;
                            }

                            // вышли за границы функций
                            if (token is MinusToken && (tokens[j + 1] is not DecimalToken &&
                                                        !parser.TokenIsMathFunctionToken(tokens[j + 1])))
                            {
                                //funcTokensEndIndex = j - 1;
                                seria.EndIndex = j - 1;
                                i = j - 1;
                                funcTokensSeries.Add(seria);
                                break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }

            var newTokensList = new List<IToken>();
            foreach (var token in tokens)
            {
                newTokensList.Add(token);
            }

            foreach (var seria in funcTokensSeries)
            {
                // 1. Рассчитываем значение функции
                var currentSeriaTokensList = 
                    tokens.GetRange(seria.StartIndex, seria.EndIndex - seria.StartIndex + 1);
                decimal funcResult = parser.CalculateMathFunctionTokens(currentSeriaTokensList);

                var funcResultToken =
                    new DecimalToken().GetTokenObject(funcResult.ToString(), tokens[seria.StartIndex].Position);

                // 2. Убираем токены, принадлежащие к этой функции
                
                // 2.1. Собираем токены, идущие до серии
                var previousTokensList = new List<IToken>();

                int startIndex = newTokensList.IndexOf(tokens[seria.StartIndex]);
                int endIndex = newTokensList.IndexOf(tokens[seria.EndIndex]);

                for (int i = 0; i < startIndex; i++)
                {
                    previousTokensList.Add(newTokensList[i]);
                }

                // 2.2. Добавляем рассчитанное значение функции
                previousTokensList.Add(funcResultToken);

                // 2.3. Добавляем оставшуюся часть
                for (int i = endIndex + 1; i < newTokensList.Count; i++)
                {
                    previousTokensList.Add(newTokensList[i]);
                }

                // 2.4. Восстанавливаем последовательность
                newTokensList.Clear();
                foreach (var token in previousTokensList)
                {
                    newTokensList.Add(token);
                }
            }
            #endregion

            #region Степени

            var debug = newTokensList;
            var powTokensSeries = new List<PowTokensSeria>();

            for (int i = 0; i < newTokensList.Count; i++)
            {
                var token = newTokensList[i];

                if (token is DecimalToken)
                {
                    if (i + 1 < newTokensList.Count - 1 && newTokensList[i + 1] is ExpToken)
                    {
                        var seria = new PowTokensSeria();
                        seria.StartIndex = i;

                        // поиск конца серии
                        for (int j = i + 1; j < tokens.Count; j++) // newTokensList[j] изначально = "^" 
                        {
                            var nextToken = newTokensList[j];

                            if (j == newTokensList.Count - 1)
                            {
                                seria.EndIndex = j;
                                powTokensSeries.Add(seria);
                                i = j;
                                break;
                            }

                            if (nextToken is MinusToken)
                            {
                                if (j + 2 < tokens.Count && newTokensList[j + 2] is not ExpToken)
                                {
                                    seria.EndIndex = j + 1;
                                    powTokensSeries.Add(seria);
                                    i = j + 1;
                                    break;
                                }
                            }

                            if (nextToken is not DecimalToken && nextToken is not MinusToken &&
                                nextToken is not ExpToken)
                            {
                                seria.EndIndex = j - 1;
                                powTokensSeries.Add(seria);
                                i = j;
                                break;
                            }

                        }

                    }
                }
            }

            var finallyTokensList = new List<IToken>();
            foreach (var token in newTokensList)
            {
                finallyTokensList.Add(token);
            }



            foreach (var seria in powTokensSeries)
            {
                var currentSeriaPowTokens =
                    newTokensList.GetRange(seria.StartIndex, seria.EndIndex - seria.StartIndex + 1);
                var currentSeriaTokensValue = parser.CalculateExpTokensValue(currentSeriaPowTokens);


                int startIndex = finallyTokensList.IndexOf(newTokensList[seria.StartIndex]);
                int endIndex = finallyTokensList.IndexOf(newTokensList[seria.EndIndex]);

                var newList = new List<IToken>();

                for (int i = 0; i < startIndex; i++)
                {
                    newList.Add(finallyTokensList[i]);
                }

                newList.Add(new DecimalToken().GetTokenObject(currentSeriaTokensValue.ToString(), finallyTokensList[startIndex].Position));

                for (int i = endIndex + 1; i < finallyTokensList.Count; i++)
                {
                    newList.Add(finallyTokensList[i]);
                }

                finallyTokensList.Clear();

                foreach (var token in newList)
                {
                    finallyTokensList.Add(token);
                }
            }

            #endregion

            var computerInputStringBuilder = new StringBuilder();
            foreach (var token in finallyTokensList)
            {
                computerInputStringBuilder.Append(token.Value + " ");
            }

            var computer = new DataTable();
            Console.WriteLine($"result: {computer.Compute(computerInputStringBuilder.ToString(), " ")}");
        }
    }
}
