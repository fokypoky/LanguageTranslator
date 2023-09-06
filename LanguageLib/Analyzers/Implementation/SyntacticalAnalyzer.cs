using System.Data;
using System.Text;
using LanguageLib.Analyzers.Interfaces;
using LanguageLib.AST.Interfaces;
using LanguageLib.Errors;
using LanguageLib.Errors.Interfaces;
using LanguageLib.Tokens.Implementation;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Implementation.MathFunctions;
using LanguageLib.Tokens.Implementation.MathOperations;
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

        public List<IVariableToken> Variables { get; set; }

        public int ErrorsCount
        {
            get => Errors.Count;
        }

        private int _operatorsStartIndex;
        private int _operatorsEndIndex;

        public SyntacticalAnalyzer(List<IToken> tokens)
        {
            Tokens = tokens;
            Errors = new List<IError>();
            Variables = new List<IVariableToken>();
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
                Errors.Add(new SyntacticalError("Токены звена не найдены", 1));
                return;
            }

            var linkTokens = new List<IToken>();

            for (int i = 1; i <= linkTokensEndIndex; i++)
            {
                linkTokens.Add(Tokens[i]);
            }
            // анализ токенов

            bool isLinkTokensSuccess = AnalyzeLinks(ref linkTokens);
            if (!isLinkTokensSuccess)
            {
                return;
            }
            #endregion

            #region Operators

            // поиск операторов

            // TODO: добавить проверку операторы ли найдены

            int operatorTokensStartIndex = linkTokensEndIndex + 1;
            int operatorTokensEndIndex = -1;

            for (int i = operatorTokensStartIndex; i < Tokens.Count; i++)
            {
                if (Tokens[i] is StopToken)
                {
                    operatorTokensEndIndex = i;
                    break;
                }
            }
            
            _operatorsStartIndex = operatorTokensStartIndex;
            _operatorsEndIndex = operatorTokensEndIndex;

            // сбор операторов

            var operatorTokens = new List<IToken>();

            for (int i = operatorTokensStartIndex; i < operatorTokensEndIndex; i++)
            {
                operatorTokens.Add(Tokens[i]);
            }
            
            // анализ операторов

            bool isOperatorTokensSuccessful = AnalyzeOperators(ref operatorTokens);
            if (!isOperatorTokensSuccessful)
            {
                return;
            }
            #endregion
        }

        public void MakeAST()
        {
            var variableNodesList = new List<IVariableAstNode>();

            // ищем индексы начала переменных
            var variablesStartIndexes = new List<int>();
            for (int i = _operatorsStartIndex; i <= _operatorsEndIndex; i++)
            {
                var token = Tokens[i];

                if (i + 1 < Tokens.Count - 1 && token is VariableToken && Tokens[i + 1] is AssignToken)
                {
                    variablesStartIndexes.Add(i);
                    i++;
                    continue;
                }

                if (TokenIsLabel(i))
                {
                    i++;
                    continue;
                }
            }


            // собираем дерево
            for (int tokenIndex = 0; tokenIndex < variablesStartIndexes.Count; tokenIndex++)
            {
                #region Определение токенов каждой переменой

                // токены текущей переменной
                var currentVariableTokensList = new List<IToken>();
                // индекс текущего токена в общем массиве
                int currentTokenGlobalIndex = variablesStartIndexes[tokenIndex];

                // собираем токены текущей переменной до начала следующей
                int nextVariableGlobalTokenIndex = -1;

                if (tokenIndex == variablesStartIndexes.Count - 1) // если текущая переменная последняя
                {
                    nextVariableGlobalTokenIndex = Tokens.Count - 2;
                }
                else // если не последняя
                {
                    int _nextVariableTokenIndex = variablesStartIndexes[tokenIndex + 1] - 1;

                    // проверка если есть метка ":" перед следующей переменной
                    if (Tokens[_nextVariableTokenIndex] is ColonToken &&
                        Tokens[_nextVariableTokenIndex - 1] is IntegerToken)
                    {
                        nextVariableGlobalTokenIndex = _nextVariableTokenIndex - 2;
                    }
                    else
                    {
                        nextVariableGlobalTokenIndex = _nextVariableTokenIndex;
                    }
                }
                // +2 потому что сначала название переменной "="
                for (int i = currentTokenGlobalIndex + 2; i <= nextVariableGlobalTokenIndex; i++)
                {
                    currentVariableTokensList.Add(Tokens[i]);
                }

                #endregion

                // Замена переменных на вещесвенные числа 

                for (int i = 0; i < currentVariableTokensList.Count; i++)
                {
                    if (currentVariableTokensList[i] is VariableToken)
                    {
                        var _token = (VariableToken)currentVariableTokensList[i];
                        foreach (var variableToken in Variables)
                        {
                            if (_token.Name == variableToken.Name)
                            {
                                currentVariableTokensList[i] =
                                    new DecimalToken().GetTokenObject(variableToken.Value, _token.Position);
                            }
                        }
                    }
                }

                
                // TODO: перевести из восьмиричной в десятичную
                var result = CalculateVariableValue(currentVariableTokensList);
                if (ErrorsCount > 0)
                {
                    return;
                }

                var variable = Tokens[currentTokenGlobalIndex];
                variable.Value = result.ToString();

                Variables.Add((IVariableToken) variable);

            }
            
        }

        private bool AnalyzeLinks(ref List<IToken> tokens)
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
                        Errors.Add(new SyntacticalError("Ожидалось вещественное число", token.Position + 1));
                        return false;
                    }

                    // проверка: третье слово - запятая
                    if ((tokenIndex + 2) < tokens.Count - 2 && tokens[tokenIndex + 2] is not CommaToken)
                    {
                        Errors.Add(new SyntacticalError("Ожидалось запятая", token.Position + 2));
                        return false;
                    }
                }
                
                else if (token is SecondToken)
                {
                    // определение конца токена
                    int secondTokenEndIndex = -1;

                    for (int i = tokenIndex; i < tokens.Count; i++)
                    {
                        var _token = tokens[i];
                        if (i == tokens.Count - 1)
                        {
                            secondTokenEndIndex = i;
                            break;
                        }

                        if (_token is CommaToken)
                        {
                            secondTokenEndIndex = i - 1;
                            break;
                        }
                    }

                    // определение токенов после слова Fourth

                    var secondWordSubTokens = new List<IToken>();

                    for (int i = tokenIndex; i <= secondTokenEndIndex; i++)
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

                else if (token is ThirdToken)
                {
                    // "Third" Целое
                    if ((tokenIndex + 1) <= tokens.Count - 1 && tokens[tokenIndex + 1] is not IntegerToken)
                    {
                        Errors.Add(new SyntacticalError("Ожидалось целое число", tokenIndex + 1));
                        return false;
                    }

                    // проверка: третье слово - запятая
                    if ((tokenIndex + 2) < tokens.Count - 2 && tokens[tokenIndex + 2] is not CommaToken)
                    {
                        Errors.Add(new SyntacticalError("Ожидалось запятая", tokenIndex + 2));
                        return false;
                    }
                }
                else if (token is FourthToken)
                {
                    // определение конца токена
                    int fourthTokenEndIndex = -1;

                    for (int i = tokenIndex; i < tokens.Count; i++)
                    {
                        var _token = tokens[i];
                        if (i == tokens.Count - 1)
                        {
                            fourthTokenEndIndex = i;
                            break;
                        }

                        if (_token is CommaToken)
                        {
                            fourthTokenEndIndex = i - 1;
                            break;
                        }
                    }

                    // определение токенов после слова Fourth

                    var fourthWordSubTokens = new List<IToken>();

                    for (int i = tokenIndex; i <= fourthTokenEndIndex; i++)
                    {
                        fourthWordSubTokens.Add(tokens[i]);
                    }

                    // проверка вещественных чисел россыпью

                    for (int i = 1; i < fourthWordSubTokens.Count; i++)
                    {
                        if (fourthWordSubTokens[i] is not IntegerToken)
                        {
                            Errors.Add(new SyntacticalError("Ожидалось целое число", fourthWordSubTokens[i].Position));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool AnalyzeOperators(ref List<IToken> tokens)
        {
            // ":", "=" может быть в конце токенов
            if (tokens[^1] is ColonToken || tokens[^1] is AssignToken)
            {
                Errors.Add(new SyntacticalError("Ожидалось объявление переменной", tokens[^1].Position));
                return false;
            }

            // проверка одна ли переменная написана перед знаком "="
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                if (token is ColonToken)
                {
                    if (i + 1 < tokens.Count - 1)
                    {
                        if (tokens[i + 1] is not VariableToken)
                        {
                            Errors.Add(new SyntacticalError("Ожидалось название переменной", tokens[i + 1].Position));
                            return false;
                        }
                    }
                    else
                    {
                        Errors.Add(new SyntacticalError("Ожидалось название переменной", tokens[i + 1].Position));
                        return false;
                    }

                    if (i + 2 < tokens.Count - 1)
                    {
                        if (tokens[i + 2] is not AssignToken)
                        {
                            Errors.Add(new SyntacticalError("Ожидалось название '='", tokens[i + 2].Position));
                            return false;
                        }
                    }
                    else
                    {
                        Errors.Add(new SyntacticalError("Ожидалось '='", token.Position + 2));
                        return false;
                    }
                }
            }

            // могут быть 2 метки и ":", после ":" может быть не переменная
            foreach (var token in tokens)
            {
                int tokenIndex = tokens.IndexOf(token);
                if (tokenIndex + 1 < tokens.Count - 1)
                {
                    var nextToken = tokens[tokenIndex + 1];
                    if (token is IntegerToken && nextToken is not ColonToken)
                    {
                        Errors.Add(new SyntacticalError("Ожидалась ':'", nextToken.Position));
                        return false;
                    }
                }
            }

            // проверка соответствия символам(sin, cos, tg, ctg, *, /, +, -, ^, переменные, метка:)
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                // определение является ли слово меткой
                if (i + 1 <= tokens.Count - 1)
                {
                    if (token is IntegerToken && tokens[i + 1] is ColonToken)
                    {
                        // i увеличивается на 1 в определении цикла
                        i++;
                        continue;
                    }
                }

                if (!TokenIsMathOperation(token) && !TokenIsMathFunctionToken(token) && token is not DecimalToken && token is not AssignToken && token is not VariableToken)
                {
                    Errors.Add(new SyntacticalError(
                        "В операторах могут быть только метка ':', вещественные числа, функции, математические операции",
                        token.Position));
                    return false;
                }
            }

            // метка может быть последним словом

            if (tokens[tokens.Count - 1] is ColonToken && tokens[tokens.Count - 2] is IntegerToken)
            {
                Errors.Add(new SyntacticalError("После ':' ожидалось определение переменной", tokens.Count - 1));
                return false;
            }


            // после знака "=" может ничего не быть("=" - последнее слово)
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is AssignToken && i == tokens.Count - 1)
                {
                    Errors.Add(new SyntacticalError("После знака '=' ожидается определение переменной", tokens[i].Position + 1));
                    return false;
                }
            }

            // переменная с одним именем может быть объявлена несколько раз

            // поиск индексов начала каждой переменной
            var variableStartIndexesList = new List<int>();

            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                if (variableStartIndexesList.Contains(i))
                {
                    continue;
                }

                // метка ":"
                if (tokens[i] is IntegerToken && i + 1 < tokens.Count - 1 && tokens[i + 1] is ColonToken)
                {
                    variableStartIndexesList.Add(i + 2);
                    continue;
                }

                // переменная "="
                if (tokens[i] is VariableToken && i + 1 < tokens.Count - 1 && tokens[i + 1] is AssignToken)
                {
                    variableStartIndexesList.Add(i);
                }
            }

            // TODO: проверить одна ли переменная объявлена

            //проверка токенов
            for (int tokenIndex = 0; tokenIndex < variableStartIndexesList.Count; tokenIndex++)
            {
                // токены текущей переменной
                var currentVariableTokensList = new List<IToken>(); 
                // индекс текущего токена в общем массиве
                int currentTokenIndex = variableStartIndexesList[tokenIndex];

                // собираем токены текущей переменной до начала следующей
                int nextVariableTokenIndex = -1;

                if (tokenIndex == variableStartIndexesList.Count - 1) // если текущая переменная последняя
                {
                    nextVariableTokenIndex = tokens.Count - 1;
                }
                else // если не последняя
                {
                    int _nextVariableTokenIndex = variableStartIndexesList[tokenIndex + 1] - 1;

                    // проверка если есть метка ":" перед следующей переменной
                    if (tokens[_nextVariableTokenIndex] is ColonToken &&
                        tokens[_nextVariableTokenIndex - 1] is IntegerToken)
                    {
                        nextVariableTokenIndex = _nextVariableTokenIndex - 2;
                    }
                    else
                    {
                        nextVariableTokenIndex = _nextVariableTokenIndex;
                    }
                }

                // начинаем с элемента после знака "="
                for (int i = currentTokenIndex + 2; i <= nextVariableTokenIndex; i++)
                {
                    currentVariableTokensList.Add(tokens[i]);
                }


                // последнее слово может быть функцией
                if (TokenIsMathFunctionToken(currentVariableTokensList[currentVariableTokensList.Count - 1]))
                {
                    Errors.Add(new SyntacticalError("Последнее слово в определении переменной - функция, а ожидался ее аргумент", currentVariableTokensList[^1].Position));
                    return false;
                }

                // последнее слово может быть математической операцией
                if (TokenIsMathOperation(currentVariableTokensList[^1]))
                {
                    Errors.Add(new SyntacticalError("Последнее слово в определении переменной - математическая операция", currentVariableTokensList[^1].Position));
                    return false;
                }

                // после вещественного числа может ничего не быть
                foreach (var token in currentVariableTokensList)
                {
                    int _tokenIndex = currentVariableTokensList.IndexOf(token);
                    if (_tokenIndex < currentVariableTokensList.Count - 1)
                    {
                        var nextToken = currentVariableTokensList[_tokenIndex + 1];
                        if (token is DecimalToken && !TokenIsMathOperation(nextToken))
                        {
                            Errors.Add(new SyntacticalError("Ожидалась математическая операция", nextToken.Position));
                            return false;
                        }
                    }
                }

                // в определении переменной могут быть либо другие переменные либо вещественные числа либо функции
                foreach (var token in currentVariableTokensList)
                {
                    // используемая переменная может быть не определена ранее
                    if (token is VariableToken)
                    {
                        var currentVariableToken = (VariableToken)token;
                        bool isVariableDefined = false;

                        for (int i = currentTokenIndex - 1; i >= 0; i--)
                        {
                            if (tokens[i] is VariableToken)
                            {
                                var findedVariableToken = (VariableToken)tokens[i];

                                if (findedVariableToken.Name == currentVariableToken.Name)
                                {
                                    isVariableDefined = true;
                                    break;
                                }
                            }
                        }

                        if (!isVariableDefined)
                        {
                            Errors.Add(new SyntacticalError($"Переменная {currentVariableToken.Name} не определена", currentVariableToken.Position));
                            return false;
                        }
                    }

                    if (!TokenFitsVariable(token))
                    {
                        Errors.Add(new SyntacticalError(
                            "В определении переменной могут содержаться только вещественные числа, переменные, функции и математические операции",
                            token.Position));
                        return false;
                    }
                }

                // проверка математических операций
                for (int i = currentTokenIndex + 2; i <= nextVariableTokenIndex; i++)
                {
                    var token = tokens[i];

                    // проверка дублирования математических операций
                    if (TokenIsMathOperation(token) && TokenIsMathOperation(tokens[i + 1]))
                    {
                        Errors.Add(new SyntacticalError("Дублирование математических операций", token.Position));
                        return false;
                    }

                    // проверка деления на 0 (предыдущее слово - "/")
                    if (token is DivisionToken && i + 1 <= tokens.Count - 1 && tokens[i + 1] is DecimalToken)
                    {
                        decimal tokenValue = Convert.ToDecimal(tokens[i + 1].Value);

                        if (tokenValue == 0)
                        {
                            Errors.Add(new SyntacticalError("Деление на 0", token.Position + 1));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public decimal CalculateVariableValue(List<IToken> tokens)
        {
            decimal result = 0;

            #region Функции

            var funcTokensSeries = new List<FuncTokensSeria>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (TokenIsMathFunctionToken(tokens[i]))
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

                        if (TokenIsMathFunctionToken(token))
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
                                                        !TokenIsMathFunctionToken(tokens[j + 1])))
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

                decimal funcResult = 0;
                try
                {
                    funcResult = CalculateMathFunctionTokens(currentSeriaTokensList);
                }
                catch (OverflowException)
                {
                    Errors.Add(new SyntacticalError("Значение функции слишком велико или слишком мало", seria.StartIndex));
                    return 0;
                }

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
                var currentSeriaTokensValue = CalculateExpTokensValue(currentSeriaPowTokens);


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

            // проверка деления на 0
            for (int i = 0; i < finallyTokensList.Count; i++)
            {
                if (finallyTokensList[i] is DivisionToken && i + 1 < tokens.Count - 1)
                {
                    var nextTokenValue = Convert.ToDecimal(finallyTokensList[i].Value);
                    if (nextTokenValue == 0)
                    {
                        Errors.Add(new SyntacticalError("Присутствует деление на 0", finallyTokensList[i + 1].Position));
                        return 0;
                    }
                }
            }

            var computer = new DataTable();
            try
            {
                var computerInputStringBuilder = new StringBuilder();
                foreach (var token in finallyTokensList)
                {
                    computerInputStringBuilder.Append(token.Value + " ");
                }
                result = (decimal)computer.Compute(computerInputStringBuilder.ToString(), " ");
            }
            catch (DivideByZeroException)
            {
                Errors.Add(new SyntacticalError("В выражении Присутствует деление на 0", 0));
            }
            catch (OverflowException)
            {
                Errors.Add(new SyntacticalError("Полученное значение слишком большое или слишком маленькое", 0));
            }

            return result;
        }

        public decimal CalculateMathFunctionTokens(List<IToken> tokens)
        {
            decimal result = 0;

            var token = tokens[0];
            if (tokens.Count == 1)
            {
                return Convert.ToDecimal(tokens[0].Value);
            }
            switch (token.Type)
            {
                // TODO: добавить степень
                //case TokenType.Exp:
                //    // получаем предыдущее значение
                //    var previousDecimalTokenValue = Convert.ToDecimal(Tokens[token.Position - 1].Value);
                //    return (decimal)Math.Pow((double)previousDecimalTokenValue,
                //        (double)CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - 1)));
                case TokenType.Cos:
                    return (decimal)Math.Cos((double)CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - 1)));
                case TokenType.Sin:
                    return (decimal)Math.Sin((double)CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - 1)));
                case TokenType.Tg:
                    return (decimal)Math.Tan((double)CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count -1)));
                case TokenType.Ctg:
                    return 1 /
                           (decimal)Math.Tan((double)CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - 1)));
                case TokenType.Minus:
                    return -1 * CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - 1));
                case TokenType.Decimal:
                    return (decimal)Math.Pow((double)Convert.ToDecimal(token.Value),
                        (double)CalculateMathFunctionTokens(tokens.GetRange(2, tokens.Count - 2)));
                    
            }

            return result;
        }

        public decimal CalculateExpTokensValue(List<IToken> tokens)
        {
            decimal result = 0;

            if (tokens.Count == 1)
            {
                return Convert.ToDecimal(tokens[0].Value);
            }

            switch (tokens[0].Type)
            {
                case TokenType.Minus:
                    result = 0 - CalculateExpTokensValue(tokens.GetRange(1, tokens.Count - 1));
                    return result;
                case TokenType.Exp:
                    result = CalculateExpTokensValue(tokens.GetRange(1, tokens.Count - 1));
                    return result;
                case TokenType.Decimal:
                    result = (decimal)Math.Pow(Convert.ToDouble(tokens[0].Value),
                        (double)CalculateExpTokensValue(tokens.GetRange(1, tokens.Count - 1)));
                    return result;
            }

            return result;
        }
        
        private bool TokenIsLinkWord(IToken token) =>
            token is FirstToken || token is SecondToken || token is ThirdToken || token is FourthToken;

        public bool TokenIsMathFunctionToken(IToken token)
        {
            return token is SinToken || token is CosToken || token is TgToken;
        }

        public bool TokenIsMathOperation(IToken token)
        {
            return token is PlusToken || token is MinusToken || token is MultiplyToken || token is DivisionToken ||
                   token is ExpToken;
        }

        private bool TokenFitsVariable(IToken token)
        {
            return TokenIsMathFunctionToken(token) || TokenIsMathOperation(token) || token is VariableToken ||
                   token is DecimalToken;
        }

        private bool TokenIsLabel(int tokenIndex)
        {
            return tokenIndex + 1 < Tokens.Count - 1 && Tokens[tokenIndex] is IntegerToken &&
                   Tokens[tokenIndex + 1] is ColonToken;
        }
    }
}
