using System.Collections.Generic;
using static WeatherPointInterest.Models.BusinessSearchEndpoint;

namespace WeatherPointInterest.Models
{
    /// <summary>
    /// Class <c>CityInfo</c> manage all the weather and points of interest info for a given city
    /// plane.</summary>
    ///
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
    
   
        /// <summary>
        /// WeatherInfo return all info about weather of city
        /// </summary>
        public WeatherInfo WeatherInfo { get { return weatherInfo; }  }

        /// <summary>
        /// BusinessSearchEndpoint return all info about business end points of city
        /// </summary>
        public BusinessSearchEndpoint BusinessSearchEndpoints { get => businessSearchEndpoint;}
        
        /// <summary>
        /// City contains info about name and geographic coordinates (lat,lon)
        /// </summary>
        public City City { get => city; }


    }
}
