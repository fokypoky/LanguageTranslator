using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LanguageLib.Analyzers.Implementation;
using LanguageLib.Errors;
using LanguageLib.Errors.Interfaces;
using LanguageLib.Tokens.Interfaces;
using LanguageUI.WPF.Domain.Repositories.Implementation;
using LanguageUI.WPF.Inftastructure.Commands;
using LanguageUI.WPF.ViewModels.Base;

namespace LanguageUI.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private ObservableCollection<IError> _errors;
        private ObservableCollection<IVariableToken> _variables;

        private string _inputText;
        private string _languageDescription;

        public string InputText
        {
            get => _inputText;
            set => SetField(ref _inputText, value);
        }

        public string LanguageDescription
        {
            get => _languageDescription;
            set => SetField(ref _languageDescription, value);
        }

        #region Public collections

        public ObservableCollection<IError> Errors
        {
            get => _errors;
            set => SetField(ref _errors, value);
        }

        public ObservableCollection<IVariableToken> Variables
        {
            get => _variables;
            set => SetField(ref _variables, value);
        }

        #endregion

        public ICommand HandleCodeCommand
        {
            get => new RelayCommand((object parameter) =>
            {
                Errors?.Clear();

                if (String.IsNullOrEmpty(InputText))
                {
                    Errors.Add(new SyntacticalError("Введенный текст пустой", 0));
                    return;
                }

                var lexer = new LexicalAnalyzer(InputText);
                lexer.Analyze();

                if (lexer.ErrorsCount > 0)
                {
                    Errors = new ObservableCollection<IError>(lexer.Errors);
                    return;
                }

                var parser = new SyntacticalAnalyzer(lexer.Tokens);
                parser.Analyze();

                if (parser.ErrorsCount > 0)
                {
                    Errors = new ObservableCollection<IError>(parser.Errors);
                    return;
                }

                parser.MakeAST();
                if (parser.ErrorsCount > 0)
                {
                    Errors = new ObservableCollection<IError>(parser.Errors);
                    return;
                }
                Variables = new ObservableCollection<IVariableToken>(parser.Variables);


            });
        }

        public MainWindowViewModel()
        {
            Errors = new ObservableCollection<IError>();
            Variables = new ObservableCollection<IVariableToken>();

            LanguageDescription = new FileRepository().ReadFile("LanguageDescription.txt");
        }

    }
}
