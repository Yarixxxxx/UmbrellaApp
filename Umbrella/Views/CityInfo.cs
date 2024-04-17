using SQLite;

public class CityInfo
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string CityName { get; set; }
    public string Country { get; set; }
    public int Population { get; set; }
    public string Landmarks { get; set; }
   
}