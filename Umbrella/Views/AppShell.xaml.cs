using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.IO;
using System.Linq;

namespace Umbrella
{
    public partial class AppShell: Shell
    {
        private SQLiteConnection database;
        public AppShell()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WeatherDatabase.db");
            database = new SQLiteConnection(databasePath);

            // Создаем таблицу для оценок пользователей, если ее еще нет
            if (!database.TableMappings.Any(m => m.MappedType.Name == nameof(Rates)))
                database.CreateTable<Rates>();
        }
    }
}