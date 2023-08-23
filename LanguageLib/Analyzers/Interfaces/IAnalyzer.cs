using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Errors.Interfaces;

namespace LanguageLib.Analyzers.Interfaces
{
    public interface IAnalyzer
    {
        List<IError> Errors { get; set; }
        int ErrorsCount { get; }
        void Analyze();
    }
}
