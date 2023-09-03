using System.Text;
using LanguageConsoleApp.Domain.Repositories.Implementation;
using LanguageLib.Analyzers.Implementation;
using LanguageLib.Errors.Implementation;
using LanguageLib.Tokens.Implementation.MathFunctions;
using LanguageLib.Tokens.Implementation.MathOperations;
using LanguageLib.Tokens.Implementation.NumberTokens;
using LanguageLib.Tokens.Interfaces;

namespace LanguageConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var repository = new FileRepository();
            string input = repository.ReadFile("InputLanguage.txt");
            
            var lexer = new LexicalAnalyzer(input);
            lexer.Analyze();

            if (lexer.ErrorsCount > 0)
            {
                Console.WriteLine("There are lexical errors\nPositions: ");
                foreach (var error in lexer.Errors)
                {
                    Console.Write(error.Position + " ");
                }

                return;
            }

            var syntaxAnalyzer = new SyntacticalAnalyzer(lexer.Tokens);
            syntaxAnalyzer.Analyze();

            if (syntaxAnalyzer.ErrorsCount > 0)
            {
                Console.WriteLine("There are syntax errors\nPositions: ");
                foreach (var error in syntaxAnalyzer.Errors)
                {
                    Console.Write(error. Position + " ");
                }

                return;
            }

            var test = new List<IToken>()
            {
                new SinToken().GetTokenObject(), new MinusToken().GetTokenObject(),
                new CosToken().GetTokenObject(), new MinusToken().GetTokenObject(),
                new CtgToken().GetTokenObject(), new DecimalToken().GetTokenObject("14.1", 0)
            };

            var test2 = new List<IToken>()
            {
                new DecimalToken().GetTokenObject("6"), new MinusToken().GetTokenObject(),
                new DecimalToken().GetTokenObject("4"), new MultiplyToken().GetTokenObject(),
                new DecimalToken().GetTokenObject("5")
            };

            Console.WriteLine(syntaxAnalyzer.CalculateMathFunctionTokens2(test2, 0));

            //syntaxAnalyzer.MakeAST();
            //var ast = syntaxAnalyzer.AST;

            Console.WriteLine("OK");
        }
    }
}