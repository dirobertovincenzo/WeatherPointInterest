using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using WeatherPointInterest.Models;

namespace WeatherPointInterest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityInfoController : ControllerBase
    {

        private readonly ILogger<CityInfoController> _logger;

        public CityInfoController(ILogger<CityInfoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityInfo>>> GetAll()
        {
            City[] cities = (new CityDAO()).GetAll();
            List<CityInfo> cityInfo = new();
            foreach (City c in cities)
            {
                cityInfo.Add(new CityInfo(c,
                this.GetWeatherInfo(c).Result,
                this.GetBusinessSearchEndpoint(c).Result
                ));
            }
            return cityInfo.ToArray();
        }
        [HttpGet("{cityName:alpha}")]
        public async Task<ActionResult<CityInfo>> GetByCityName(string name)
        {
            City city = (new CityDAO()).Get(name);
            if (city == null)
            {
                return NotFound();
            }
            return this.GetCityInfo(city);
        }

        [HttpGet("{cityId}")]
        public async Task<ActionResult<CityInfo>> GetByCityId(int cityId)
        {
            City city = (new CityDAO()).Get(cityId);
            if (city == null)
            {
                return NotFound();
            }
            return this.GetCityInfo(city);
        }

        private CityInfo GetCityInfo(City city)
        { 
            CityInfo cityInfo = new CityInfo(city,
                this.GetWeatherInfo(city).Result,
                this.GetBusinessSearchEndpoint(city).Result
                );
            return cityInfo;
        }


        //Hosted web API REST Service base url

        private async Task<WeatherInfo> GetWeatherInfo(City city)
        {
            WeatherInfo weather = new WeatherInfo();
            using (var client = new HttpClient())
            {
                string baseUrl = "https://api.openweathermap.org/";
                //Passing service base url
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Converting Request Params to Key Value Pair.  
                List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                allIputParams.Add(new KeyValuePair<string, string>("lat", city.Latitude.ToString()));
                allIputParams.Add(new KeyValuePair<string, string>("lon", city.Longitude.ToString()));
                allIputParams.Add(new KeyValuePair<string, string>("cnt", "1"));
                allIputParams.Add(new KeyValuePair<string, string>("appid", "194ab5351af797b6bbda6cf1ee5fc321"));
                string requestParams = string.Empty;

                // URL Request Query parameters.  
                requestParams = new FormUrlEncodedContent(allIputParams).ReadAsStringAsync().Result;

                WeatherInfo weatherInfo = null;

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient
                HttpResponseMessage Res = await client.GetAsync("data/2.5/forecast/?" + requestParams);
                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    string result = Res.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the Employee list
                    weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result);
                }
                return weatherInfo;
            }
        }

        private async Task<BusinessSearchEndpoint> GetBusinessSearchEndpoint(City city)
        {
            BusinessSearchEndpoint businessSearchEndpoint = null;
            var clientHandler = new HttpClientHandler();
            using (var client = new HttpClient(clientHandler))
            {
                string baseUrl = "https://api.yelp.com/";
                //Passing service base url
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "vwCUDDoUaABrpUUoVUXCltwuYsPGKrGy5Hu5E9G_QuK3HHsw0-NRu9EPd8luHt6Gbvv6wQtukLtauHstHKoeZ7WdwjOcNuSuCwG7Pg0BFZbxtQIhBYudK_YfNr5KY3Yx");
                // Converting Request Params to Key Value Pair.  
                List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();

                //Force double parameter to use "." separator because the endpoint doesn't accept ","
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                allIputParams.Add(new KeyValuePair<string, string>("latitude", city.Latitude.ToString(nfi)));
                allIputParams.Add(new KeyValuePair<string, string>("longitude", city.Longitude.ToString(nfi)));
                string requestParams = string.Empty;

                // URL Request Query parameters.  
                requestParams = new FormUrlEncodedContent(allIputParams).ReadAsStringAsync().Result;
                //Sending request to find web api REST service resource using HttpClient
                HttpResponseMessage Res = await client.GetAsync("v3/businesses/search?" + requestParams);
                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    string result = Res.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the BusinessSearchEndpoint list
                    businessSearchEndpoint = JsonConvert.DeserializeObject<BusinessSearchEndpoint>(result);
                }

                //returning the businessSearchEndpoint list
                return businessSearchEndpoint;

            }
        }
    }
}
