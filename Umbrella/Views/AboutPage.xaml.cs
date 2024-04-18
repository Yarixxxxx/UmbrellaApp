using System;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Collections.ObjectModel;
using OfficeOpenXml;
using System.Net.Http;

using Xamarin.Essentials;
using Xamarin.Forms;

using SQLite;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbrella.Services;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using static Umbrella.Views.AboutPage;
using System.Net;
using OfficeOpenXml.Core.ExcelPackage;

namespace Umbrella.Views
{
    public partial class AboutPage : ContentPage
    {
        static string API = "970c06777ff5ac394aa923ebf74cabf6";
        private string username;
        ObservableCollection<City> cityItems;

        public static List<CityData> citiesData;
        private static HttpClient client = new HttpClient();
        public User CurrentUser { get; set; }
        SQLiteConnection database;
        static string temp;
        static string humidity;
        static string speed;
        public static string russianName = "Пермь";
        private UserDataService userDataService;
        private User currentUser;
        bool temperatureChanged = TemperatureChanged;
        int count = 0;
        private ObservableCollection<UserFavoriteCity> favoriteCities;
        ObservableCollection<UserFavoriteCity> favoriteCityNames;
        static List<CityNames> cities = new List<CityNames>();

        public static bool TemperatureChanged { get; set; }
        public static bool WindChanged { get; set; }

        public AboutPage()
        {
            InitializeComponent();
            GetLocation();

            InitializeDatabase();
            PopulateDatabaseWithTranslations();
            PopulateDatabaseWithCities();
            string filePath = $"https://raw.githubusercontent.com/Yarixxxxx/WeatherPhotos/main/place_cities_3%20(1).json";
            
            LoadCitiesFromUrl(filePath, listView);

            LoadUserSettings(LoginPage.LoggedInUserId);
            LoadWeatherDataForCity("perm");

            userDataService = new UserDataService();
            currentUser = userDataService.GetCurrentUser(LoginPage.LoggedInUserId, database);


        }
        protected override void OnAppearing()
        {
            LoadWeatherDataForCity(Town.Text);
        }

        public class CityData
        {
            public string Name { get; set; }
            public string nameEn { get; set; }
            public string displayName { get; set; }
            public string addressCity { get; set; }
            public string addressCounty { get; set; }
            public string addressState { get; set; }
            public string addressIv4 { get; set; }
            public string addressRegion { get; set; }
            public string addressPostcode { get; set; }
            public string addressCountry { get; set; }
            public string addressCountryCode { get; set; }
            public int? Population { get; set; }
        }

        public class CityNames
    {
        public string NameEn { get; set; }
        public string NameRu { get; set; }
    }



        //public static async Task LoadCitiesFromUrl(string url, ListView listView)
        //{
        //    try
        //    {
        //        HttpResponseMessage response = await client.GetAsync(url);
        //        response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешен

        //        string jsonContent = await response.Content.ReadAsStringAsync();

        //        // Десериализация JSON в список объектов CityNames
        //        citiesData = JsonConvert.DeserializeObject<List<CityNames>>(jsonContent);

        //        // Настройка привязки данных в ListView
        //        listView.ItemsSource = citiesData;
        //        var dataTemplate = new DataTemplate(() =>
        //        {
        //            var label = new Label();
        //            label.SetBinding(Label.TextProperty, "NameRu"); // Устанавливаем привязку к свойству NameRu
        //            label.TextColor = Color.White; // Устанавливаем белый цвет текста
        //            label.FontSize = 16; // Устанавливаем размер шрифта
        //            label.FontFamily = "Arkhip"; // Устанавливаем шрифт
        //            label.HorizontalTextAlignment = TextAlignment.Center;
        //            var stackLayout = new StackLayout
        //            {
        //                BackgroundColor = (Color)Application.Current.Resources["Primary"], // Устанавливаем фон из ресурсов
        //                HorizontalOptions = LayoutOptions.FillAndExpand,
        //                VerticalOptions = LayoutOptions.FillAndExpand,
        //                Padding = new Thickness(10)
        //            };
        //            stackLayout.Children.Add(label);

        //            return new ViewCell { View = stackLayout };
        //        }); // Отображение русских названий городов
        //        listView.ItemTemplate = dataTemplate;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        // Обработка ошибки при загрузке данных по URL
        //        Console.WriteLine($"HTTP error occurred: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Обработка других ошибок при загрузке данных по URL
        //        Console.WriteLine($"Error loading data from URL: {ex.Message}");
        //    }
        //}

        public static async Task LoadCitiesFromUrl(string url, ListView listView)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешен

                string jsonContent = await response.Content.ReadAsStringAsync();

                // Десериализация JSON в список объектов CityNames
                citiesData = JsonConvert.DeserializeObject<List<CityData>>(jsonContent);

                // Отфильтровать список, исключив элементы со значением null
                var filteredCitiesData = citiesData.Where(city => city != null).ToList();

                // Установить отфильтрованный список в ItemsSource
                listView.ItemsSource = filteredCitiesData;

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
                }); // Отображение русских названий городов
                listView.ItemTemplate = dataTemplate;
            }
            catch (HttpRequestException ex)
            {
                // Обработка ошибки при загрузке данных по URL
                Console.WriteLine($"HTTP error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Обработка других ошибок при загрузке данных по URL
                Console.WriteLine($"Error loading data from URL: {ex.Message}");
            }
        }


        private static async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            CityData selectedCity = e.Item as CityData;
            russianName = selectedCity.Name;
            await GetWeatherData(selectedCity.nameEn.ToLower()); // Передача английского названия в качестве параметра
        }

        private static async Task GetWeatherData(string city)
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API}&units=metric";
                string response = await client.GetStringAsync(url);
                JObject json = JObject.Parse(response);
                string temperature = json["main"]["temp"].ToString();
                string humidity = json["main"]["humidity"].ToString();
                string windSpeed = json["wind"]["speed"].ToString();

                string message = $"Погода в {russianName}: \nТемпература  {temperature}°C,\nВлажность {humidity}%,\nСкорость ветра {windSpeed} m/s";

                // Вывод сообщения с данными о погоде
                await Application.Current.MainPage.DisplayAlert("Данные о погоде", message, "OK");
            }
            catch (HttpRequestException ex)
            {
                // Обработка ошибки при выполнении HTTP запроса
                await Application.Current.MainPage.DisplayAlert("Error", $"HTTP error occurred: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                // Обработка других ошибок
                await Application.Current.MainPage.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }



        private async void GetLocation()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();
                if (location != null)
                {
                    double latitude = location.Latitude;
                    double longitude = location.Longitude;
                    var userLocation = new Locations
                    {
                        UserId = currentUser.Id.ToString(),
                        Location = latitude.ToString() + " " + longitude.ToString(), 
                        DateTime = DateTime.Now 
                    };

                    database.Insert(userLocation);
                    DisplayAlert("Геолокация", "Ваши координаты: " + latitude + " " + longitude, "OK");
                    // Теперь у вас есть широта и долгота пользователя, которые вы можете использовать в вашем приложении
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибки получения локации
            }
        }

        private void LoadFavoriteCities()
        {
            if (currentUser != null)
            {
                // Получение избранных городов для текущего пользователя
                favoriteCities = new ObservableCollection<UserFavoriteCity>(userDataService.GetFavoriteCities(currentUser.Id));
                // Проверка наличия элемента favoriteCitiesListView перед его использованием

            }
        }

        private void AddToFavoritesClicked(object sender, EventArgs e)
        {
            if (currentUser != null)
            {
                // Получение выбранного города
                string selectedTown = Town.Text; // Замените на реальное название города или получите его из элемента управления
                                                                    // Добавление выбранного города в избранные города текущего пользователя
                userDataService.AddFavoriteCity(currentUser.Id.ToString(), selectedTown);
                // Обновление списка избранных городов

            }
        }

        public Settings LoadUserSettings(int userId)
        {
            return database.Table<Settings>().FirstOrDefault(s => s.UserId == userId);
        }


        public void SaveUserSettings(int userId, string temperature, string windSpeed)
        {
            // Проверяем, существуют ли настройки для данного пользователя
            var existingSettings = database.Table<Settings>().FirstOrDefault(s => s.UserId == userId);

            // Если настройки существуют, удаляем их
            if (existingSettings != null)
            {
                database.Delete(existingSettings);
            }

            // Создаем новые настройки и записываем их
            var newSettings = new Settings
            {
                UserId = userId,
                Temperature = temperature,
                WindSpeed = windSpeed
            };
            database.Insert(newSettings);
        }


        public async Task LoadWeatherDataForCity(string city)
        {
            string API = "970c06777ff5ac394aa923ebf74cabf6";
            string temperatureUnits = "metric";
            string windSpeedUnits = "metric";

            // Если только один параметр равен true, то другой будет metric
            

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API}&units={temperatureUnits}";
                    string response = await client.GetStringAsync(url);
                    var json = JObject.Parse(response);
                    temp = json["main"]["temp"].ToString();
                    humidity = json["main"]["humidity"].ToString();
                    speed = json["wind"]["speed"].ToString();

                    if (ItemsPage.IsFahrengeit)
                    {
                        temperatureUnits = "imperial";
                        url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API}&units={temperatureUnits}";
                        response = await client.GetStringAsync(url);
                        json = JObject.Parse(response);
                        temp = json["main"]["temp"].ToString();
                    }
                    if (ItemsPage.IsMiles)
                    {
                        temperatureUnits = "imperial";
                        url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API}&units={temperatureUnits}";
                        response = await client.GetStringAsync(url);
                        json = JObject.Parse(response);
                        speed = json["wind"]["speed"].ToString();
                    }
                    // Обновляем интерфейс с полученными данными
                    UpdateUIWithDataForCity(city, temp, humidity, speed);
                    SaveUserSettings(LoginPage.LoggedInUserId, temperatureUnits, windSpeedUnits);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
        }


        private void UpdateUIWithDataForCity(string city, string temperature, string humidity, string windSpeed)
        {

            //// Получаем время из словаря
            //var cityTime = database.Table<CityTime>().Where(c => c.Town == city).FirstOrDefault();

            //DateTime utcNow = DateTime.UtcNow;
            //int hours;
            //DateTime realTime = DateTime.Now;

            //if (int.TryParse(cityTime.Time, out hours))
            //{
            //    realTime = utcNow.AddHours(hours);
            //}

            //string formattedTime = realTime.ToString("HH:mm");

            Town.Text = russianName.ToUpper().Substring(0,1) + russianName.Substring(1);
            Town2.Text = russianName.ToUpper().Substring(0, 1) + russianName.Substring(1);

            if (ItemsPage.IsFahrengeit)
            {
                Temp.Text = temperature + " °F";
                Temp2.Text = temperature + " °F";
            }
            else if (ItemsPage.IsCelsius || !ItemsPage.IsFahrengeit)
            {
                Temp.Text = temperature + " °C";
                Temp2.Text = temperature + " °C";
            }
                 
            if (ItemsPage.IsMiles)
            {
                Wind.Text = windSpeed + " миль/ч";
                Wind2.Text = windSpeed + " миль/ч";
                Wind.TranslationX = -80; 
                Wind2.TranslationX = -80;
            }
            else if (ItemsPage.IsMetres || !ItemsPage.IsMiles)
            {
                Wind.Text = windSpeed + " м/с";     
                Wind2.Text = windSpeed + " м/с";
                Wind.TranslationX = -109; 
                Wind2.TranslationX = -109;
            }
            
            Humidity.Text = humidity + " %";
            Humidity2.Text = humidity + " %";
            //Time.Text = formattedTime;
            //Time2.Text = formattedTime;
        }





        // Метод для инициализации базы данных
        private void InitializeDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WeatherDatabase.db");
            database = new SQLiteConnection(databasePath);

            // Проверяем существование таблиц перед их созданием
            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(City)))
                database.CreateTable<City>();
            // Проверяем существование таблиц перед их созданием
            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(City)))
                database.CreateTable<Cities>();

            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(CityTime)))
                database.CreateTable<CityTime>();

            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(CityInfo)))
                database.CreateTable<CityInfo>();

            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(CityTranslation)))
                database.CreateTable<CityTranslation>();

            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(Rates)))
                database.CreateTable<Rates>();

            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(Locations)))
                database.CreateTable<Locations>();

            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(Activities)))
                database.CreateTable<Activities>();

            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(Settings)))
                database.CreateTable<Settings>();
            // Создаем таблицу для пользователей, если ее еще нет
            //if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(User)))
            //    database.CreateTable<User>();
        }


        // Метод для загрузки избранных городов из базы данных
        // Метод для сохранения избранных городов для данного пользователя в базе данных
        private void SaveFavoriteCitiesToDatabase(string userId)
        {
            // Очистка данных избранных городов в базе данных перед сохранением
            database.Execute($"DELETE FROM UserFavoriteCity WHERE UserId = ?", userId);

            // Сохранение всех избранных городов для данного пользователя в базу данных
            foreach (var favoriteCity in favoriteCities)
            {
                if (favoriteCity.UserId == userId)
                {
                    database.Insert(favoriteCity);
                }
            }
        }

        private void LoadFavoriteCities(int userId)
        {
            string userID = userId.ToString();
            var favoriteCityNames = database.Table<UserFavoriteCity>()
                               .Where(favCity => favCity.UserId == userID)
                               .ToList();

            favoriteCities = new ObservableCollection<UserFavoriteCity>(favoriteCityNames);
            //foreach (var city in favoriteCityNames)
            //{
            //    favoriteCities.Add(city);
            //}

            // Обновляем источник данных для привязки в пользовательском интерфейсе

        }

        public void AddToFavorites(string userId, string town)
        {

            if (!string.IsNullOrEmpty(town))
            {
                favoriteCities.Add(new UserFavoriteCity { UserId = userId, NameRu = town });
                database.Insert(new UserFavoriteCity { UserId = userId, NameRu = town }); // Добавление в базу данных

                // Обновляем источник данных для привязки в пользовательском интерфейсе
                listView.ItemsSource = favoriteCities;
            }
        }

        public void RemoveFromFavorites(string userId, string town)
        {
            var existingCity = favoriteCities.FirstOrDefault(c => c.UserId == userId && c.NameRu == town);
            if (existingCity != null)
            {
                favoriteCities.Remove(existingCity);
                database.Table<UserFavoriteCity>().Delete(favCity => favCity.UserId == userId && favCity.NameRu == town);

                // Обновляем источник данных для привязки в пользовательском интерфейсе
                listView.ItemsSource = favoriteCities;
            }
        }

        public void DeleteAllFavoriteCities(string userId)
        {
            // Удаление всех избранных городов для данного пользователя из базы данных
            database.Execute($"DELETE FROM UserFavoriteCity WHERE UserId = ?", userId);

            // Очищаем коллекцию
            favoriteCities.Clear();

            // Обновляем источник данных для привязки в пользовательском интерфейсе
            listView.ItemsSource = favoriteCities;
        }


        // Метод для удаления всех записей из таблицы избранных городов
        //private void DeleteAllFavoriteCitiesFromDatabase(string userId)
        //{
        //    // Удаляем все записи из таблицы UserFavoriteCity, где UserId равен указанному значению
        //    database.Table<UserFavoriteCity>().Delete(favCity => favCity.UserId == userId);

        //    // После удаления всех записей, вы также можете обновить коллекцию favoriteCities,
        //    // чтобы она была синхронизирована с базой данных
        //    LoadFavoriteCities(currentUser.GetUserId());
        //}



        private async void OnChangeCityClicked(object sender, EventArgs e)
        {
            var citySelectionPage = new CitySelectionPage();
            citySelectionPage.CitySelected += OnCitySelected;
            await Navigation.PushModalAsync(new NavigationPage(citySelectionPage));
        }

        public class CityWeatherInfo
        {
            public string SelectedCity { get; set; }
            public string Temperature { get; set; }
            public string Humidity { get; set; }
            public string WindSpeed { get; set; }
            public string Time {  get; set; }
        }

        private void OnCitySelected(object sender, object args)
        {
            if (args is CityWeatherInfo cityWeatherInfo)
            {
                // Обновляем текст в StackLayout на странице AboutPage
                Town.Text = cityWeatherInfo.SelectedCity;
                Town2.Text = cityWeatherInfo.SelectedCity;
                Temp.Text = cityWeatherInfo.Temperature;
                Temp2.Text = cityWeatherInfo.Temperature;
                Wind.Text = cityWeatherInfo.WindSpeed;
                Wind2.Text = cityWeatherInfo.WindSpeed;
                Humidity.Text = cityWeatherInfo.Humidity;
                Humidity2.Text = cityWeatherInfo.Humidity;
                //Time.Text = cityWeatherInfo.Time.ToString();
                //Time2.Text = cityWeatherInfo.Time.ToString();
            }
        }


        private void PopulateDatabaseWithCities()
        {
            //if (database.Table<CityInfo>().Any())
            //    return;

            var citiesInfo = new List<CityInfo>
    {
        new CityInfo { CityName = "Пермь", Country = "Russia", Population = 0, Landmarks = "Theatre" },
        new CityInfo { CityName = "Москва", Country = "Russia", Population = 0, Landmarks = "Kremlin" },
        new CityInfo { CityName = "Калуга", Country = "Russia", Population = 0, Landmarks = "Cathedral" },
        new CityInfo { CityName = "Петербург", Country = "Russia", Population = 0, Landmarks = "Hermitage" },
        new CityInfo { CityName = "Казань", Country = "Russia", Population = 0, Landmarks = "Kremlin" },
        new CityInfo { CityName = "Нью-Йорк", Country = "USA", Population = 0, Landmarks = "Statue of Liberty" },
        new CityInfo { CityName = "Мурманск", Country = "Russia", Population = 0, Landmarks = "Port" },
    };
            foreach (var cityInfo in citiesInfo)
            {
                database.Insert(cityInfo);
            }
        }



        private void LoadCitiesFromDatabase()
        {
            var citiesFromDB = database.Table<City>().ToList();
            cityItems = new ObservableCollection<City>(citiesFromDB.Select(city =>
                new City { Town = city.Town, Image = city.Image, EnglishName = city.EnglishName, District = city.District, Subject = city.Subject, Latitude = city.Latitude, Longitude = city.Longitude, Population = city.Population }));
            listView.ItemsSource = cityItems;
        }


        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            // Реализация поиска по городам
            var searchText = e.NewTextValue.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Если поле поиска пустое, отображаем все города
                listView.ItemsSource = citiesData;
            }
            else
            {
                // Фильтрация списка городов по введенному тексту
                var matchedItems = citiesData.Where(item => item.Name.ToLower().Contains(searchText));

                if (matchedItems.Any())
                {
                    // Если есть совпадения, отображаем их
                    listView.ItemsSource = matchedItems;
                }
                else
                {
                    // Если совпадений нет, выводим сообщение "Города нет!"
                    listView.ItemsSource = new List<string> { "Города нет!" };
                }
            }
        }


        private async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            string cityName = char.ToUpper(Town.Text[0]) + Town.Text.Substring(1); // Получаем текст из Label

            // Находим информацию о городе в списке
            var cityData = citiesData.FirstOrDefault(ci => ci.Name.ToLower() == cityName.ToLower());

            if (cityData != null)
            {
                // Перенаправляем пользователя на страницу с информацией о городе
                await Navigation.PushAsync(new CityInfoPage(cityData));
            }
            else
            {
                await DisplayAlert("Ошибка", "Информация о городе не найдена.", "OK");
            }
        }







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


        

        public void RemoveFavoriteCity(string userId, string cityName)
        {
            var favoriteCity = database.Table<UserFavoriteCity>().FirstOrDefault(c => c.UserId == userId && c.NameRu == cityName);
            if (favoriteCity != null)
            {
                database.Delete(favoriteCity);
            }
        }

        public List<string> GetFavoriteCities(string userId)
        {
            var favoriteCities = database.Table<UserFavoriteCity>().Where(c => c.UserId == userId).Select(c => c.NameRu).ToList();
            return favoriteCities;
        }


        // Переменная для отслеживания текущего выбранного города
      

        // Метод для обновления интерфейса пользователя с новыми данными о городе
        // Метод для обновления интерфейса пользователя с новыми данными о городе


        // Обработчик события для кнопки "Следующий город"
        // Обработчик события для кнопки "Следующий город"

        private int currentCityIndex = 0; // Инициализация переменной currentCityIndex

        private void NextCityClicked(object sender, EventArgs e)
        {
            if (currentUser != null)
            {
            
                LoadFavoriteCities(currentUser.Id);

                if (favoriteCities == null || favoriteCities.Count == 0)
                {
                  
                    return;
                }

         
                currentCityIndex++;

       
                if (currentCityIndex >= favoriteCities.Count)
                {
                    currentCityIndex = 0;
                }

                UserFavoriteCity selectedCity = favoriteCities[currentCityIndex];
                russianName = selectedCity.NameRu;
         
                LoadWeatherDataForCity(selectedCity.NameRu);
            }
        }

        private void PreviousCityClicked(object sender, EventArgs e)
        {
            if (currentUser != null)
            {
       
                LoadFavoriteCities(currentUser.Id);

                if (favoriteCities == null || favoriteCities.Count == 0)
                {
                 
                    return;
                }


                currentCityIndex--;

       
                if (currentCityIndex < 0)
                {
                    currentCityIndex = favoriteCities.Count - 1; 
                }

       
                UserFavoriteCity selectedCity = favoriteCities[currentCityIndex];
                russianName = selectedCity.NameRu;
       
                LoadWeatherDataForCity(selectedCity.NameRu);
            }
        }





        private void PopulateDatabaseWithTranslations()
        {
            foreach (var translation in cityTranslations)
            {

                var existingTranslation = database.Table<CityTranslation>().FirstOrDefault(t => t.EnglishName == translation.Key);
                if (existingTranslation == null)
                {

                    database.Insert(new CityTranslation { EnglishName = translation.Key, RussianName = translation.Value });
                }
                else
                {

                    existingTranslation.RussianName = translation.Value;
                    database.Update(existingTranslation);
                }
            }
        }

        public interface IPath
        {
            string GetDatabasePath(string filename);
        }


    }

    
}