using SQLite;
using System.Collections.Generic;

public class City
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Town { get; set; }
    public string EnglishName { get; set; }
    public string District { get; set; }
    public int Population { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Subject { get; set; }
    public string Image { get; set; }

}
