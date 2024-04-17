using SQLite;
using System;
using System.IO;

using System.Linq;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Net.Http;

using Xamarin.Essentials;
using Xamarin.Forms;

using Newtonsoft.Json.Linq;
using Umbrella.Services;
using System.Windows.Input;
using System.Threading.Tasks;



namespace Umbrella.Views
{

    public partial class ItemDetailPage : ContentPage
    {
        private SQLiteConnection database;

        public ItemDetailPage()
        {
            InitializeComponent();
        }

        private void InitializeDatabase()
        {
            if (database == null)
            {
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WeatherDatabase.db");
                database = new SQLiteConnection(databasePath);

                // Создаем таблицу для оценок пользователей, если ее еще нет
                if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(Rates)))
                    database.CreateTable<Rates>();
            }
        }

        private async void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            // Получаем оценку от пользователя


            // Получаем ID текущего пользователя (это может быть ваша логика, например, использование AuthService)
            string userId = "your_user_id_logic_here";

            // Создаем новый объект оценки
            var rate = new Rates
            {
                UserId = userId,
                DateTime = DateTime.Now
            };

            // Сохраняем оценку в базе данных
            database.Insert(rate);

            // Показываем пользователю сообщение об успешном сохранении оценки
            await DisplayAlert("Успех", "Ваше замечание учтено. Спасибо!", "OK");

            // Возвращаемся на предыдущую страницу
            await Navigation.PopAsync();
        }
    }
}