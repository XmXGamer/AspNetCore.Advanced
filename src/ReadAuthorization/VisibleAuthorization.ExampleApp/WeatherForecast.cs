using System;
using AspNetCore.Advanced.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace ExampleApp
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        [VisibleAuthorize]
        public string Summary { get; set; }
    }
}
