using SQLite;
using System;
using System.Collections.Generic;

public class Locations
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Location { get; set; }

    public DateTime DateTime { get; set; }

}