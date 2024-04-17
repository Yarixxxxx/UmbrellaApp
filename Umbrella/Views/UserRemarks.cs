using SQLite;
using System;
using System.Collections.Generic;

public class UserRemarks
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Message { get; set; }
    public string RemarkType { get; set; }
    public DateTime Timestamp { get; set; }

}
