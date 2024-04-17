using SQLite;

public class UserFavoriteCity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string UserId { get; set; } 
    public string NameRu { get; set; } 

}
