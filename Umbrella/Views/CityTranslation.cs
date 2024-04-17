using SQLite;

public class CityTranslation
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string EnglishName { get; set; }
    public string RussianName { get; set; }
}
