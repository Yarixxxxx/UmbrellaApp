using SQLite;

public class CityTime
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Town { get; set; } 
    public string Time { get; set; } 
}

