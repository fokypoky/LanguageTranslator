using System.Collections.ObjectModel;
using System.Windows.Input;
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
