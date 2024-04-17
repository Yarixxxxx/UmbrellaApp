using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using static Xamarin.Essentials.Permissions;
using System.Net.Http;
using static Umbrella.Views.AboutPage;

namespace Umbrella.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CitySelectionPage : ContentPage
    {
        ObservableCollection<City> cityItems;
        SQLiteConnection database;
        public CitySelectionPage()
        {
            InitializeComponent();
            InitializeDatabase();
            PopulateDatabaseWithTranslations();
            PopulateDatabaseWithCities();
            PopulateDatabaseWithTimes();
            LoadCitiesFromDatabase();
            LoadTimesFromDatabase();
        }
        // В классе CitySelectionPage добавьте новое событие
        public event EventHandler<object> CitySelected;

        // В методе OnItemTapped вызывайте событие с информацией о погоде при выбранном городе
        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null && e.Item is CityData cityItem)
            {
                string town = cityItem.nameEn.ToLower();
                russianName = cityItem.Name.ToLower();
                string API = "970c06777ff5ac394aa923ebf74cabf6";

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string url = $"https://api.openweathermap.org/data/2.5/weather?q={town}&appid={API}&units=metric";

                        string response = await client.GetStringAsync(url);
                        var json = JObject.Parse(response);
                        string temp = json["main"]["temp"].ToString();
                        string humidity = json["main"]["humidity"].ToString();
                        string speed = json["wind"]["speed"].ToString();

                        town = GetRussianCityName(town);

                        // Получаем время из словаря
                        //var cityTime = database.Table<CityTime>().Where(c => c.Town == town).FirstOrDefault();

                        //DateTime utcNow = DateTime.UtcNow;
                        //int hours;
                        //DateTime realTime = DateTime.Now;

                        //if (int.TryParse(cityTime.Time, out hours))
                        //{
                        //    realTime = utcNow.AddHours(hours);
                        //}
                                                
                        //string formattedTime = realTime.ToString("HH:mm");
                        
                        // Вызываем событие и передаем информацию о выбранном городе и погоде
                        CitySelected?.Invoke(this, new CityWeatherInfo
                        {
                            SelectedCity = town,
                            Temperature = temp + " °C",
                            Humidity = humidity + " %",
                            WindSpeed = speed + " м/с",
                            //Time = formattedTime
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    
                }
                await Navigation.PopModalAsync();
            }
        }


        private void InitializeDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WeatherDatabase.db");
            database = new SQLiteConnection(databasePath);
            database.CreateTable<City>();
            database.CreateTable<CityTime>();
            database.CreateTable<CityTranslation>(); // Добавляем новую таблицу для переводов
        }

        //public event EventHandler<string> CitySelected;

        //private async void OnLocationChanged(object sender, ItemTappedEventArgs e)
        //{
        //    if (e.Item != null && e.Item is City cityItem)
        //    {
        //        string town = cityItem.Town;

        //        // Вызываем событие и передаем название выбранного города
        //        CitySelected?.Invoke(this, town);
        //    }


        //    await Navigation.PopModalAsync(); // Закрыть модальное окно после выбора города
        //}

        private Dictionary<string, string> cityTranslations = new Dictionary<string, string>()
{
    { "Perm", "Пермь" },
    { "Moscow", "Москва" },
    { "Kaluga", "Калуга" },
    { "Petersburg", "Петербург" },
    { "Kazan", "Казань" },
    { "New York", "Нью-Йорк" },
    { "Murmansk", "Мурманск" }
};

        // Метод для получения русского названия города по английскому
        private string GetRussianCityName(string englishCityName)
        {
            var translation = database.Table<CityTranslation>().FirstOrDefault(t => t.EnglishName == englishCityName);
            if (translation != null)
            {
                return translation.RussianName;
            }
            else
            {
                // Если перевода нет в базе данных, возвращаем исходное английское название
                return englishCityName;
            }
        }



        public static Dictionary<string, string> cityTimesDictionary = new Dictionary<string, string>();
        private void PopulateDatabaseWithTimes()
        {
            var cities = new[]
            {
                new CityTime { Town = "Пермь", Time = "5" },
                new CityTime { Town = "Москва", Time = "3" },
                new CityTime { Town = "Калуга", Time = "3" },
                new CityTime { Town = "Петербург", Time = "3" },
                new CityTime { Town = "Казань", Time = "3" },
                new CityTime { Town = "Нью-Йорк", Time = "-4" },
                new CityTime { Town = "Мурманск", Time = "3" },
            };

            foreach (var city in cities)
            {
                // Проверяем, существует ли город в базе данных
                var existingCity = database.Table<CityTime>().FirstOrDefault(c => c.Town == city.Town);
                if (existingCity == null)
                {
                    // Если города нет, добавляем его в базу данных
                    database.Insert(city);
                }
                else
                {
                    // Если город уже существует, обновляем информацию о нем
                    existingCity.Time = city.Time;
                    database.Update(existingCity);
                }
            }
        }

        private void PopulateDatabaseWithTranslations()
        {
            foreach (var translation in cityTranslations)
            {
                // Проверяем, существует ли уже перевод для этого города в базе данных
                var existingTranslation = database.Table<CityTranslation>().FirstOrDefault(t => t.EnglishName == translation.Key);
                if (existingTranslation == null)
                {
                    // Если перевода нет, добавляем его в базу данных
                    database.Insert(new CityTranslation { EnglishName = translation.Key, RussianName = translation.Value });
                }
                else
                {
                    // Если перевод уже существует, можно обновить его, если это необходимо
                    existingTranslation.RussianName = translation.Value;
                    database.Update(existingTranslation);
                }
            }
        }

        private void PopulateDatabaseWithCities()
        {
            if (database.Table<City>().Any())
                return;

            var cities = new[]
            {
new City { Town = "Perm", Image = "perm.jpg"},
new City { Town = "Moscow", Image = "https://github.com/Yarixxxxx/WeatherPhotos/blob/main/moscow.jpg?raw=true" },
new City { Town = "Kaluga", Image = "https://github.com/Yarixxxxx/WeatherPhotos/blob/main/kaluga.jpg?raw=true" },
new City { Town = "Petersburg", Image = "https://github.com/Yarixxxxx/WeatherPhotos/blob/main/petersburg.jpg?raw=true" },
new City { Town = "Kazan", Image = "https://github.com/Yarixxxxx/WeatherPhotos/blob/main/kazan.jpg?raw=true" },
new City { Town = "New York", Image = "https://github.com/Yarixxxxx/WeatherPhotos/blob/main/new%20york.jpg?raw=true" },
new City { Town = "Murmansk", Image = "https://github.com/Yarixxxxx/WeatherPhotos/blob/main/murmansk.jpg?raw=true" },
};

            foreach (var city in cities)
            {
                database.Insert(city);

                // Добавление информации о городе в таблицу CityInfo
                var cityInfo = new CityInfo
                {
                    CityName = city.Town,
                    Country = "Russia", // Здесь можно указать соответствующую страну
                    Population = 0 // Здесь можно указать население города
                };
                database.Insert(cityInfo);

            }
        }


        private void LoadCitiesFromDatabase()
        {
            var citiesFromDB = database.Table<City>().ToList();
            cityItems = new ObservableCollection<City>(citiesFromDB.Select(city =>
                new City { Town = GetRussianCityName(city.Town), Image = city.Image }));
            listViewLocation.ItemsSource = citiesData;
            var dataTemplate = new DataTemplate(() =>
            {
                var label = new Label();
                label.SetBinding(Label.TextProperty, "Name"); // Устанавливаем привязку к свойству NameRu
                label.TextColor = Color.White; // Устанавливаем белый цвет текста
                label.FontSize = 16; // Устанавливаем размер шрифта
                label.FontFamily = "Arkhip"; // Устанавливаем шрифт
                label.HorizontalTextAlignment = TextAlignment.Center;
                var stackLayout = new StackLayout
                {
                    BackgroundColor = (Color)Application.Current.Resources["Primary"], // Устанавливаем фон из ресурсов
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(10)
                };
                stackLayout.Children.Add(label);

                return new ViewCell { View = stackLayout };
            }); ; // Отображение русских названий городов
            listViewLocation.ItemTemplate = dataTemplate;
        }


        private void LoadTimesFromDatabase()
        {
            // Загрузка данных из базы данных и отображение их в списке
            var cityTimeFromDB = database.Table<CityTime>().ToList();
           
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            // Реализация поиска по городам
            var searchText = e.NewTextValue.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                listViewLocation.ItemsSource = citiesData;
            }
            else
            {
                var matchedItems = citiesData.Where(item => item.Name.ToLower().Contains(searchText));

                if (matchedItems.Any())
                {
                    listViewLocation.ItemsSource = matchedItems;
                }
                else
                {
                    listViewLocation.ItemsSource = new List<string> { "Города нет!" };
                }
            }
        }
    }
}