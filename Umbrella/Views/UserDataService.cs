
using Xamarin.Essentials;

using SQLite;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Umbrella.Services
    {
        public class UserDataService
        {
            readonly SQLiteConnection database;

        public UserDataService()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WeatherDatabase.db");
            database = new SQLiteConnection(databasePath);
            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(User)))
                database.CreateTable<User>();
            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(UserFavoriteCity)))
                database.CreateTable<UserFavoriteCity>();
        }

        public User GetCurrentUser(int id, SQLiteConnection database)
        {
            // Получаем пользователя из базы данных по его имени пользователя
            return database.Table<User>().FirstOrDefault(u => u.Id == id);
        }

        public List<UserFavoriteCity> GetFavoriteCities(int userId)
        {
            // Получаем избранные города пользователя из базы данных по его идентификатору
            return database.Table<UserFavoriteCity>().Where(c => c.UserId == userId.ToString()).ToList();
        }

        public List<User> GetAllUsers()
        {
            // Получаем всех пользователей из базы данных
            var users = database.Table<User>().ToList();
            return users;
        }

        public bool RegisterUser(User user)
        {
            // Проверка наличия пользователя в базе данных
            var existingUser = database.Table<User>().FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                return false; // Пользователь уже существует
            }

            // Сохранение нового пользователя в базе данных
            database.Insert(user);
            return true;
        }

        private void AddDefaultFavoriteCities(string userId)
        {
            // Добавление двух избранных городов для нового пользователя
            database.Insert(new UserFavoriteCity { UserId = userId, NameRu = "Москва" });
            database.Insert(new UserFavoriteCity { UserId = userId, NameRu = "Санкт-Петербург" });
        }


        public bool AuthenticateUser(string username, string password, out int userId)
        {
            // Проверка существования пользователя и правильности пароля
            var user = database.Table<User>().FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                userId = user.Id; // Присваиваем идентификатор пользователя
                return true; // Возвращаем true, если пользователь найден
            }
            else
            {
                userId = default(int); // Устанавливаем userId в значение по умолчанию для int
                return false; // Возвращаем false, если пользователь не найден
            }
        }

        public User GetUserIdByUsername(string id)
        {
            // Пытаемся преобразовать id в целое число
            if (!int.TryParse(id, out int userId))
            {
                // Если не удалось преобразовать, возвращаем null или выбрасываем исключение, в зависимости от вашей логики
                return null;
            }

            // Возвращаем пользователя по его Id
            return database.Table<User>().FirstOrDefault(u => u.Id == userId);
        }

        public void AddFavoriteCity(string userId, string town)
        {
            database.Insert(new UserFavoriteCity { UserId = userId, NameRu = town });
        }


    }
}
