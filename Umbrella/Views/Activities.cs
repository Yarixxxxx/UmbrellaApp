using SQLite;
using System;
using System.Collections.Generic;

public class Activities
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string NameOfActivity { get; set; }

    public DateTime DateTime {  get; set; }
}
