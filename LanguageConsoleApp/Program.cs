using System.Text;
using LanguageConsoleApp.Domain.Repositories.Implementation;
using LanguageLib.Analyzers.Implementation;
using LanguageLib.Errors.Implementation;

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

            Console.WriteLine("OK");
        }
    }
}