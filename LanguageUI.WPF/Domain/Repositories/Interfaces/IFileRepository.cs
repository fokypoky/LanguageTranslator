using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageUI.WPF.Domain.Repositories.Interfaces
{
    public interface IFileRepository
    {
        string ReadFile(string filePath);
        void WriteFile(string filePath, string content);
    }
}
