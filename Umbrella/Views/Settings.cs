using SQLite;
using System.Collections.Generic;

public class Settings
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Temperature { get; set; }
    public string WindSpeed { get; set; }

}
