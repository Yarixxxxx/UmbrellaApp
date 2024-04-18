using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.IO;
namespace Umbrella.Views
{
    public partial class ItemsPage : ContentPage
    {
        public static bool IsCelsius { get; set; }
        public static bool IsMetres { get; set; }
        public static bool IsFahrengeit { get; set; }
        public static bool IsMiles { get; set; }
        SQLiteConnection database;
        public static RadioButton button = new RadioButton();
        public ItemsPage()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadUserSettings();
        }

        private void InitializeDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WeatherDatabase.db");
            database = new SQLiteConnection(databasePath);
        }

        protected override void OnAppearing()
        {
            LoadUserSettings();
        }


        private void LoadUserSettings()
        {
            // Получаем идентификатор текущего пользователя (предположим, что он доступен как свойство в классе LoginPage)
            int userId = LoginPage.LoggedInUserId;

            // Получаем настройки пользователя из базы данных
            var userSettings = database.Table<Settings>().FirstOrDefault(s => s.UserId == userId);

                // Применяем настройки к RadioButton
            celsiusRadioButton.IsChecked = userSettings.Temperature == "metric";
            fahrenheitRadioButton.IsChecked = userSettings.Temperature == "imperial";
            metersPerSecondRadioButton.IsChecked = userSettings.WindSpeed == "metric";
            kilometersPerHourRadioButton.IsChecked = userSettings.WindSpeed == "imperial";
           

        }

        private void OnTemperatureUnitChanged(object sender, CheckedChangedEventArgs e)
        {
            // Обработка изменения выбора единиц температуры
            if (celsiusRadioButton.IsChecked)
            {
                IsCelsius = true;
                IsFahrengeit = false;
            }
            else if (fahrenheitRadioButton.IsChecked)
            {
                IsCelsius = false;
                IsFahrengeit = true;
            }
        }

        private void OnWindSpeedUnitChanged(object sender, CheckedChangedEventArgs e)
        {
            // Обработка изменения выбора единиц скорости ветра
            if (metersPerSecondRadioButton.IsChecked)
            {
                IsMetres = true;
                IsMiles = false;
            }
            else if (kilometersPerHourRadioButton.IsChecked)
            {
                IsMiles = true;
                IsMetres = false;
            }
        }
    }
}
