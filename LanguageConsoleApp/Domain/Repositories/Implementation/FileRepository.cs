using LanguageConsoleApp.Domain.Repositories.Interfaces;

namespace LanguageConsoleApp.Domain.Repositories.Implementation
{
    public class FileRepository : IFileRepository
    {
        public string ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            using (var streamReader = new StreamReader(filePath))
            {
                return streamReader.ReadToEnd();
            }
        }

        public void WriteFile(string filePath, string content)
        {
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                }
            }
        }
    }
}
