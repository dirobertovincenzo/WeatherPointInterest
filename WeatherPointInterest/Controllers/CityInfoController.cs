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
        //Interfaccia per accesso a appjson
        private readonly IConfiguration _configuration;
        //Istanzia dinamicamente HttpClient provando a sopperire il problema del consuming delle socket
        private readonly IHttpClientFactory _httpClientFactory;



        public CityInfoController(ILogger<CityInfoController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        
        [HttpGet]
        public async Task<ActionResult<CityInfo[]>> GetAll()
        {
            City[] cities = (new CityDAO()).GetAll();
            if(cities.Length == 0)
            {
                return NotFound();
            }
            List<CityInfo> cityInfo = new();
            foreach (City c in cities)
            {

                cityInfo.Add(await this.GetCityInfo(c)); 
            }
            return cityInfo.ToArray();
        }
        //Permette di accedere ad una determinata città tramite parametro nome
        //Dicitura alpha permette di definire che il valore in ingresso è di tipo stringa
        [HttpGet("{cityName:alpha}")]
        public async Task<ActionResult<CityInfo>> GetByCityName(string cityName)
        {
            City city = (new CityDAO()).Get(cityName);
            if (city == null)
            {
                return NotFound();
            }
            return await this.GetCityInfo(city);
        }

        [HttpGet("{cityId}")]
        //Permette di accedere ad una determinata città tramite parametro id di tipo int
        public async Task<ActionResult<CityInfo>> GetByCityId(int cityId)
        {
            City city = (new CityDAO()).Get(cityId);
            if (city == null)
            {
                return NotFound();
            }
            return await this.GetCityInfo(city);
        }

        private async Task<CityInfo> GetCityInfo(City city)
        { 
            CityInfo cityInfo = new CityInfo(city,
                await this.GetWeatherInfo(city),
                await this.GetBusinessSearchEndpoint(city)
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
                //Parametro cnt permette di definire il numero di estrazioni di meteo per intervalli di tempo
                allIputParams.Add(new KeyValuePair<string, string>("cnt", _configuration["WeatherCntDefault"]));
                allIputParams.Add(new KeyValuePair<string, string>("appid", _configuration["WeatherAPIKey"]));
                string requestParams = string.Empty;

                // URL Request Query parameters.  
                requestParams = new FormUrlEncodedContent(allIputParams).ReadAsStringAsync().Result;

                WeatherInfo? weatherInfo = null;

                //Sending request to find web api REST service using HttpClient
                HttpResponseMessage Res = await client.GetAsync("data/2.5/forecast/?" + requestParams);
                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    string result = Res.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the weatherInfo parameter
                    weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result);
                }
#pragma warning disable CS8603 // Possibile restituzione di riferimento Null.
                return weatherInfo;
#pragma warning restore CS8603 // Possibile restituzione di riferimento Null.
            }
        }

        private async Task<BusinessSearchEndpoint> GetBusinessSearchEndpoint(City city)
        {
            BusinessSearchEndpoint? businessSearchEndpoint = null;            
            using (var client = _httpClientFactory.CreateClient("Business"))
            {
                // Converting Request Params to Key Value Pair.  
                List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();

                //Force double parameter to use "." separator because the endpoint doesn't accept ","
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                allIputParams.Add(new KeyValuePair<string, string>("latitude", city.Latitude.ToString(nfi)));
                allIputParams.Add(new KeyValuePair<string, string>("longitude", city.Longitude.ToString(nfi)));
                //Permette di definire il numero
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
