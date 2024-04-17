using SQLite;
using System;
using System.Collections.Generic;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

public class Rates
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Rate { get; set; }
    public DateTime DateTime { get; set; }
}