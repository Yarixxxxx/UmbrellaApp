using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Collections.ObjectModel;

using System.Net.Http;

using Xamarin.Essentials;

using Newtonsoft.Json.Linq;
using Umbrella.Services;
using System.Windows.Input;
namespace Umbrella.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Rate : ContentPage
    {
        private User currentUser;
        SQLiteConnection database;
        private UserDataService userDataService;


        public Rate()
        {
            InitializeComponent();
            InitializeDatabase();

        }

        private void InitializeDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WeatherDatabase.db");
            database = new SQLiteConnection(databasePath);

            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(Rates)))
                database.CreateTable<Rates>();
            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(UserRemarks)))
                database.CreateTable<UserRemarks>();
            // Создаем таблицу для пользователей, если ее еще нет
            //if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(User)))
            //    database.CreateTable<User>();
        }

        private async void OnSubmitButtonClicked(object sender, EventArgs e)
        {

            int rating = (int)ratingSlider.Value;

            string userId = LoginPage.LoggedInUserId.ToString();

            var lastRating = database.Table<Rates>().Where(r => r.UserId == userId).OrderByDescending(r => r.DateTime).FirstOrDefault();

            if (lastRating != null)
            {
                var timeDifference = DateTime.Now - lastRating.DateTime;

                if (timeDifference.TotalDays < 60)
                {
                    await DisplayAlert("Ошибка", "Вы уже оставляли оценку. Сможете сделать это вновь после следующего обновления!", "OK");
                    return; 
                }
            }

            database.Insert(new Rates { UserId = userId, Rate = rating.ToString(), DateTime = DateTime.Now });

            await DisplayAlert("Успех", "Ваша оценка поставлена. Спасибо!", "OK");

            await Navigation.PopAsync();
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            // Получаем ID текущего пользователя
            string userId = LoginPage.LoggedInUserId.ToString();

            // Получаем текст из поля ввода
            string message = remarksEntry.Text;

            // Создаем новый объект пользовательского сообщения
            var userRemark = new UserRemarks
            {
                UserId = userId,
                Message = message,
                RemarkType = "General", // По умолчанию тип сообщения - общий
                Timestamp = DateTime.Now // Сохраняем текущую дату и время
            };

            // Вставляем новое сообщение в базу данных
            database.Insert(userRemark);

            // Показываем пользователю сообщение об успешной отправке
            await DisplayAlert("Успех", "Ваше сообщение отправлено. Спасибо!", "OK");

            // Очищаем поле ввода после отправки сообщения
            remarksEntry.Text = "";
        }


    }
}