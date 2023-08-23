using System.Text;
using LanguageConsoleApp.Domain.Repositories.Implementation;
using LanguageLib.Analyzers.Implementation;
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
                foreach (var lexerError in lexer.Errors)
                {
                    Console.WriteLine(lexerError?.ToString());
                }

                return;
            }

            foreach (var token in lexer.Tokens)
            {
                Console.WriteLine(token.Value);
            }

        }
    }
}