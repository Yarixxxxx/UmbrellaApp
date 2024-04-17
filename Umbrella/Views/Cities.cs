using SQLite;
using System.Collections.Generic;

public class Cities
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Town { get; set; }
    public string DisplayName { get; set; }
    public int Population { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Image { get; set; }

}
