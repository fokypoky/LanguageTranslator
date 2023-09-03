using System.Data;
using System.Runtime.InteropServices;
using LanguageLib.Analyzers.Interfaces;
using LanguageLib.AST.Implementation.Nodes.MathOperations;
using LanguageLib.AST.Implementation.Nodes.NumericValues;
using LanguageLib.AST.Implementation.Nodes.Root;
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
using LanguageLib.AST.Implementation.Nodes.Variables;
using LanguageLib.AST.Interfaces;

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

        private int _operatorsStartIndex;
        private int _operatorsEndIndex;

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

            var variableNodes = new List<IVariableAstNode>();

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
                    nextVariableGlobalTokenIndex = Tokens.Count - 1;
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

                /*var currentVariableToken = (VariableToken)Tokens[currentTokenGlobalIndex];
                
                var currentVariableNode = new VariableASTNode(currentVariableToken.Name);

                currentVariableNode.Value = CalculateVariableTokens(currentVariableTokensList);

                variableNodes.Add(currentVariableNode);
                */

                // TODO: сделать замену переменной (в определении) на вещественное число

                decimal variableValue = CalculateMathFunctionTokens2(currentVariableTokensList, 0);

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
            // TODO: реализовать

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
                    
                    // проверка дублирования математических операций(**, //, ^^, ++)
                    if (TokenIsMathOperation(token))
                    {
                        var leftToken = tokens[i - 1];
                        var rightToken = tokens[i + 1];

                        // дублирование математических операций
                        if ((TokenIsMathOperation(leftToken) || TokenIsMathOperation(rightToken)) && (leftToken.Type == token.Type || rightToken.Type == token.Type) &&
                            (token.Type != TokenType.Minus))
                        {
                            Errors.Add(new SyntacticalError("Дублирование математических операций", token.Position));
                            return false;
                        }

                        // если математические операции разные, кроме +-
                        if (TokenIsMathOperation(leftToken) && leftToken.Type != TokenType.Plus &&
                            token.Type != TokenType.Minus && token.Type != leftToken.Type)
                        {
                            Errors.Add(new SyntacticalError("Различные математические операции, разрешено '+-' ", token.Position));
                            return false;
                        }

                        if (TokenIsMathOperation(rightToken) && rightToken.Type != TokenType.Minus &&
                            token.Type != TokenType.Minus)
                        {
                            Errors.Add(new SyntacticalError("Различные математические операции, разрешено '+-' ", token.Position));
                            return false;
                        }
                    }

                    // проверка на то, что между математической операцией что-то есть

                    if (TokenIsMathOperation(token) && i > 0 && i < tokens.Count - 1)
                    {
                        var leftToken = tokens[i - 1];
                        var rightToken = tokens[i + 1];

                        if (!TokenIsMathOperation(leftToken) && TokenIsMathFunctionToken(leftToken) &&
                            leftToken is not DecimalToken && leftToken is not VariableToken && !(token is MinusToken && leftToken is AssignToken))
                        {
                            Errors.Add(new SyntacticalError("Ожидалась переменная, вещественное число, функция или математическая операция", leftToken.Position));
                            return false;
                        }

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

        //private decimal CalculateVariableTokens(List<IToken> tokens, bool isDivMultPart)
        //// isDivMultPart - для рекурсивного вызова при наличии деления или умножения
        //{
        //    // TODO: ИДЕМ ЛИНЕЙНО, ЧЕРЕЗ SWITCH-CASE, ПРОВЕРЯЕМ ОПЕРАЦИИ УМНОЖЕНИЯ И ДЕЛЕНИЯ ТОЖЕ ЛИНЕЙНО
        //    // TODO: С ШАГОМ ЧЕРЕЗ 1
        //    decimal result = 0;

        //    for (int i = 0; i < tokens.Count; i++)
        //    {
        //        var token = tokens[i];

        //        switch (token.Type)
        //        {
        //            // если первое слово - вещественное число
        //            case TokenType.Decimal:
        //                result += Convert.ToDecimal(token.Value);
        //                break;

        //            #region Математические операции

        //            case TokenType.Multiply:
        //                // выделяем часть с делением/умножением
                        
        //                if (isDivMultPart) // если функция вызвана рекурсивно
        //                {
        //                    // TODO: здесь будет основной расчет, вызывается в рекурсии

        //                    break;
        //                }

        //                int multiplyPartEndIndex = -1;

        //                for (int j = i + 1; j < tokens.Count; j++)
        //                {
        //                    var nextToken = tokens[j];

        //                    if (TokenIsMathOperation(nextToken) &&
        //                        (nextToken is not DivisionToken && nextToken is not MultiplyToken))
        //                    {
        //                        multiplyPartEndIndex = j - 1;
        //                    }
        //                }

        //                if (multiplyPartEndIndex == -1) // если конечный индекс не нашелся, то блок с умножением - последний
        //                {
        //                    multiplyPartEndIndex = tokens.Count - 1;
        //                }

        //                // собираем токены для рекурсивного вызова функции
        //                var recursionTokens = new List<IToken>();
                        
        //                for (int j = i; j <= multiplyPartEndIndex; j++)
        //                {
        //                    recursionTokens.Add(tokens[j]);
        //                }

        //                result += CalculateVariableTokens(recursionTokens, true);

        //                break;

        //            case TokenType.Division:
                        
        //                if (isDivMultPart)
        //                {
        //                    // TODO: здесь будет основной расчет, вызывается в рекурсии
        //                    break;
        //                }



        //                break;

        //            case TokenType.Exp:
        //                break;

        //            case TokenType.Plus:
        //                break;

        //            case TokenType.Minus:
        //                break;

        //            #endregion

        //            #region Математические функции

        //            case TokenType.Sin:
        //                break;

        //            case TokenType.Cos:
        //                break;

        //            case TokenType.Tg:
        //                break;

        //            case TokenType.Ctg:
        //                break;

        //            #endregion
        //        }
        //    }

        //    return result;
        //}

        //private decimal CalculateDivMultTokens(List<IToken> tokens)
        //{
        //    decimal result = 1;

        //    for (int i = 0; i < tokens.Count; i++)
        //    {
        //        var token = tokens[i];

        //        switch (token.Type) // могут быть только функции, числа, переменные, операции умножения и деления
        //        {
        //            case TokenType.Sin:
        //                break;
                    
        //            case TokenType.Cos:
        //                break;

        //            case TokenType.Tg: 
        //                break;

        //            case TokenType.Ctg:
        //                break;

        //            case TokenType.Division:
        //                break;

        //            case TokenType.Multiply:
        //                var rigthToken = tokens[i + 1];
        //                if (TokenIsMathFunctionToken(rigthToken) || rigthToken is MinusToken) // возможна серия функций (sin cos cos -tg ctg 12.3)
        //                {
        //                    // может быть отрицательная функция (например sin -cos20)
        //                    // собираем список токенов функции
        //                    int functionTokensEndIndex = -1;
        //                    for (int j = i + 1; j < tokens.Count; j++)
        //                    {
        //                        var _token = tokens[j];

        //                        if (!TokenIsMathFunctionToken(_token) && _token is not MinusToken)
        //                        {
        //                            functionTokensEndIndex = j;
        //                        }
        //                    }

        //                    // собираем список функций
        //                    var functionTokensList = new List<IToken>();
        //                    for (int j = i + 1; j <= functionTokensEndIndex; j++)
        //                    {
        //                        functionTokensList.Add(tokens[j]);
        //                    }

        //                    result *= CalculateMathFunctionTokens(functionTokensList);

        //                    i = functionTokensEndIndex + 1;
        //                    break;

        //                }

        //                if (rigthToken is DecimalToken)
        //                {
        //                    result *= Convert.ToDecimal(rigthToken.Value);
        //                    i++;
        //                    break;
        //                }

        //                break;

        //            case TokenType.Decimal:
        //                decimal tokenValue = Convert.ToDecimal(token.Value);

        //                if (i == 0) // если вещественное число - первое в списке
        //                {
        //                    result = tokenValue;
        //                    break;
        //                }

        //                if (i == 1 && tokens[0] is MinusToken) // первое слово может быть минусом
        //                {
        //                    result = 0 - tokenValue;
        //                    break;
        //                }

        //                var leftToken = tokens[i - 1];
        //                // слева может быть функция
        //                if (TokenIsMathFunctionToken(leftToken))
        //                {
        //                    // если функция - первое слово
        //                    if (i - 1 == 0)
        //                    {
        //                        result = GetTokenFunctionValue(leftToken, tokenValue);
        //                        break;
        //                    }

        //                    // TODO: здесь может быть ошибка
                            
        //                    #warning Обязательно ли += ???...
        //                    result += GetTokenFunctionValue(leftToken, tokenValue);
        //                    break;

        //                }
        //                // слева знак умножения
        //                if (leftToken is MultiplyToken)
        //                {
        //                    result *= Convert.ToDecimal(token.Value);
        //                    break;
        //                }
        //                // слева знак деления
        //                if (leftToken is DivisionToken)
        //                {
        //                    result /= Convert.ToDecimal(token.Value);
        //                    break;
        //                }
        //                break;
        //        }
        //    }

        //    return result;
        //}


        //private decimal GetTokenFunctionValue(IToken token, decimal value)
        //{
        //    switch (token.Type)
        //    {
        //        case TokenType.Cos:
        //            return (decimal)Math.Cos(Convert.ToDouble(token.Value));

        //        case TokenType.Sin:
        //            return (decimal)Math.Sin(Convert.ToDouble(token.Value));

        //        case TokenType.Tg:
        //            return (decimal)Math.Tan(Convert.ToDouble(token.Value));

        //        case TokenType.Ctg:
        //            return 1 / (decimal)Math.Tan(Convert.ToDouble(token.Value));

        //    }

        //    return 0;
        //}

        public decimal CalculateMathFunctionTokens(List<IToken> tokens)
        {
            decimal result = 0;

            var token = tokens[0];

            switch (token.Type)
            {
                case TokenType.Cos:
                    return (decimal)Math.Cos((double)CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - 1)));
                case TokenType.Sin:
                    return (decimal)Math.Sin((double)CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - 1)));
                case TokenType.Tg:
                    return (decimal)Math.Tan((double)CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - -1)));
                case TokenType.Ctg:
                    return 1 /
                           (decimal)Math.Tan((double)CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - 1)));
                case TokenType.Minus:
                    return -1 * CalculateMathFunctionTokens(tokens.GetRange(1, tokens.Count - 1));
                case TokenType.Decimal:
                    return Convert.ToDecimal(token.Value);
            }

            return result;
        }


        public decimal CalculateMathFunctionTokens2(List<IToken> tokens, decimal result)
        {
            var token = tokens[0];

            // последнее слово - обязательно вещественное число(переменная заменяется числом в блоке выше)
            if (tokens.Count == 1)
            {
                return Convert.ToDecimal(token.Value);
            }

            var nextRecursiveList = tokens.GetRange(1, tokens.Count - 1);

            // TODO: реализовать порядок математических операций
            switch (token.Type)
            {
                case TokenType.Cos:
                    result = (decimal)Math.Cos((double)CalculateMathFunctionTokens2(nextRecursiveList, result));
                    return result;

                case TokenType.Sin:
                    result = (decimal)Math.Sin((double)CalculateMathFunctionTokens2(nextRecursiveList, result));
                    return result;

                case TokenType.Tg:
                    result = (decimal)Math.Tan((double)CalculateMathFunctionTokens2(nextRecursiveList, result));
                    return result;

                case TokenType.Ctg:
                    result =  1 /
                           (decimal)Math.Tan((double)CalculateMathFunctionTokens2(nextRecursiveList, result));
                    return result;

                case TokenType.Minus:
                    result = -1 * CalculateMathFunctionTokens2(nextRecursiveList, result);
                    return result;

                case TokenType.Plus:
                    result = CalculateMathFunctionTokens2(nextRecursiveList, result);
                    return result;
                
                case TokenType.Division:
                    result = 1 / CalculateMathFunctionTokens2(nextRecursiveList, result);
                    return result;
                
                case TokenType.Multiply:
                    result = 1 * CalculateMathFunctionTokens2(nextRecursiveList, result);
                    return result;

                case TokenType.Decimal:
                    // TODO: не должна выходить из рекурсии
                    
                    result = Convert.ToDecimal(token.Value) + CalculateMathFunctionTokens2(nextRecursiveList, result);
                    return result;
            }

            return result;
        }

        private bool TokenIsLinkWord(IToken token) =>
            token is FirstToken || token is SecondToken || token is ThirdToken || token is FourthToken;

        private bool TokenIsMathFunctionToken(IToken token)
        {
            return token is SinToken || token is CosToken || token is TgToken || token is CtgToken;
        }

        private bool TokenIsMathOperation(IToken token)
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
