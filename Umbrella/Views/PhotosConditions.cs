using SQLite;
using System.Collections.Generic;

public class PhotosConditions
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Condition { get; set; }
    public string Image { get; set; }

}
