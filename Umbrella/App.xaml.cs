using Xamarin.Forms;
using Umbrella.Views;

namespace Umbrella
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Устанавливаем AppShell в качестве главной страницы
            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
