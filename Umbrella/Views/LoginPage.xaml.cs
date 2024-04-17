using SQLite;
using System;
using Umbrella.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.IO;
using System.Linq;

namespace Umbrella.Views
{
    public partial class LoginPage : ContentPage
    {
        SQLiteConnection database;
        private UserDataService userDataService = new UserDataService();
        private User currentUser; // Добавляем поле currentUser
        public static int LoggedInUserId { get; private set; }

        public LoginPage()
        {
            InitializeComponent();
            InitializeDatabase();
            var activityLog = new Activities
            {
                UserId = LoggedInUserId,
                NameOfActivity = "Login",
                DateTime = DateTime.Now
            };
            database.Insert(activityLog);
            userDataService = new UserDataService();
            // Присваиваем переданный объект user переменной currentUser
        }

        private void InitializeDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WeatherDatabase.db");
            database = new SQLiteConnection(databasePath);

            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(Activities)))
                database.CreateTable<Activities>();
            // Создаем таблицу для пользователей, если ее еще нет
            //if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(User)))
            //    database.CreateTable<User>();
        }

        private async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            // Получаем введенные пользователем данные из элементов Entry
            string username = usernameEntry.Text;
            string password = passwordEntry.Text;

            // Проверяем, заполнены ли поля
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка", "Введите имя пользователя и пароль", "OK");
                return;
            }

            // Создаем новый объект пользователя
            var newUser = new User
            {
                Username = username,
                Password = password
            };

            // Регистрируем нового пользователя
            if (userDataService.RegisterUser(newUser))
            {
                await DisplayAlert("Успех", "Пользователь успешно зарегистрирован", "OK");
            }
            else
            {
                await DisplayAlert("Ошибка", "Пользователь с таким именем уже существует", "OK");
            }
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            // Получаем введенные пользователем данные из элементов Entry
            string username = usernameEntry.Text;
            string password = passwordEntry.Text;

            // Проверяем, заполнены ли поля
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка", "Введите имя пользователя и пароль", "OK");
                return;
            }

            // Аутентификация пользователя
            if (userDataService.AuthenticateUser(username, password, out int userId))
            {
                await DisplayAlert("Успех", "Вход выполнен успешно", "OK");

                // Создание новой страницы Shell
                var appShell = new AppShell();

                Application.Current.MainPage = appShell;

                LoggedInUserId = userId; // Сохраняем идентификатор пользователя
            }

            else
            {
                await DisplayAlert("Ошибка", "Неверное имя пользователя или пароль", "OK");
            }
        }

    }
}
