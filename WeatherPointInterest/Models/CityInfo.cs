using System.Collections.Generic;
using static WeatherPointInterest.Models.BusinessSearchEndpoint;

namespace WeatherPointInterest.Models
{
    public class CityInfo
    {
        private readonly City city;
        private readonly WeatherInfo weatherInfo;
        private readonly BusinessSearchEndpoint businessSearchEndpoint;

        public CityInfo(City city, WeatherInfo weatherInfo, BusinessSearchEndpoint businessSearchEndpoints)
        {
            this.weatherInfo = weatherInfo;
            this.city = city;
            this.businessSearchEndpoint = businessSearchEndpoints;
          
        }
    
   

        public WeatherInfo WeatherInfo { get { return weatherInfo; }  }
        public BusinessSearchEndpoint BusinessSearchEndpoints { get => businessSearchEndpoint;}

        public City City { get => city; }


    }
}
