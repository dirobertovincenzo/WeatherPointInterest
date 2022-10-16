using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Dynamic;
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
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public CityInfoController(ILogger<CityInfoController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
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
        public async Task<ActionResult<CityInfo>> GetByCityName(string cityName)
        {
            City city = (new CityDAO()).Get(cityName);
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
            using (var client = _httpClientFactory.CreateClient("Weather"))
            {
                // Converting Request Params to Key Value Pair.  
                List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                allIputParams.Add(new KeyValuePair<string, string>("lat", city.Latitude.ToString()));
                allIputParams.Add(new KeyValuePair<string, string>("lon", city.Longitude.ToString()));
                allIputParams.Add(new KeyValuePair<string, string>("cnt", _configuration["WeatherCntDefault"]));
                allIputParams.Add(new KeyValuePair<string, string>("appid", _configuration["WeatherAPIKey"]));
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
            using (var client = _httpClientFactory.CreateClient("Business"))
            {
                // Converting Request Params to Key Value Pair.  
                List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();

                //Force double parameter to use "." separator because the endpoint doesn't accept ","
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                allIputParams.Add(new KeyValuePair<string, string>("latitude", city.Latitude.ToString(nfi)));
                allIputParams.Add(new KeyValuePair<string, string>("longitude", city.Longitude.ToString(nfi)));
                allIputParams.Add(new KeyValuePair<string, string>("limit", _configuration["BusinessLimit"]));
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
