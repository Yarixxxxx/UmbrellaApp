using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Umbrella.Views;
using Xamarin.Forms;

namespace Umbrella.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(ExecuteLoginCommand);
        }

        private void ExecuteLoginCommand()
        {
            // Действие, которое должно выполняться при выборе элемента меню "Login"
        }
    }
}
