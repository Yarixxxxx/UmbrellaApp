using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.Views
{
    public class WeatherData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string City { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
    }

}
