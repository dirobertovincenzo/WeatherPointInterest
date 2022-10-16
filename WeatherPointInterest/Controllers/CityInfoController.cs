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


        //Interfaccia per accesso a appjson
        private readonly IConfiguration _configuration;
        //Istanzia dinamicamente HttpClient provando a sopperire il problema del consuming delle socket
        private readonly IHttpClientFactory _httpClientFactory;



        public CityInfoController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Returns all the info on all the cities available in the system
        /// </summary>
        /// <returns>City[]</returns>
        [HttpGet]
        public async Task<ActionResult<CityInfo[]>> GetAll()
        {
            City[] cities = await (new CityDAO()).GetAll();
            if (cities.Length == 0)
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
            City city = await (new CityDAO()).Get(cityName);
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
            City city = await (new CityDAO()).Get(cityId);
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

        [HttpGet("{id}/weather/{cntResult?}")]
        public async Task<ActionResult<WeatherInfo>> GetWeatherOfCity(int id, int cntResult = 0)
        {
            City city = await (new CityDAO()).Get(id);
            if (city == null)
            {
                return NotFound();
            }

            return await this.GetWeatherInfo(city, cntResult);
        }
        [HttpGet("{cityName:alpha}/weather/{cntResult?}")]
        public async Task<ActionResult<WeatherInfo>> GetWeatherOfCity(string cityName, int cntResult = 0)
        {
            City city = await (new CityDAO()).Get(cityName);
            if (city == null)
            {
                return NotFound();
            }
            return await this.GetWeatherInfo(city, cntResult);
        }

        [HttpGet("{id}/business/{limit?}")]
        public async Task<ActionResult<BusinessSearchEndpoint>> GetBusinessOfCity(int id, int limit = 0)
        {
            City city = await (new CityDAO()).Get(id);
            if (city == null)
            {
                return NotFound();
            }

            return await this.GetBusinessSearchEndpoint(city, limit);
        }
        [HttpGet("{cityName:alpha}/business/{limit?}")]
        public async Task<ActionResult<BusinessSearchEndpoint>> GetBusinessOfCity(string cityName, int limit = 0)
        {
            City city = await (new CityDAO()).Get(cityName);
            if (city == null)
            {
                return NotFound();
            }
            return await this.GetBusinessSearchEndpoint(city, limit);
        }





        private async Task<WeatherInfo> GetWeatherInfo(City city, int cntResult = 0)
        {
            WeatherInfo weather = new WeatherInfo();
            using (var client = _httpClientFactory.CreateClient("Weather"))
            {
                // Converting Request Params to Key Value Pair.  
                List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();

                //Force double parameter to use "." separator because the endpoint doesn't accept ","
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                allIputParams.Add(new KeyValuePair<string, string>("lat", city.Latitude.ToString(nfi)));
                allIputParams.Add(new KeyValuePair<string, string>("lon", city.Longitude.ToString(nfi)));

                //Parametro cnt permette di definire il numero di estrazioni di meteo per intervalli di tempo
                if (cntResult == 0)
                {
                    Int32.TryParse(_configuration["WeatherCntDefault"], out cntResult);
                }

                allIputParams.Add(new KeyValuePair<string, string>("cnt", cntResult.ToString()));
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
                    if (!String.IsNullOrEmpty(result)){
                        //Deserializing the response recieved from web api and storing into the weatherInfo parameter
                        weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result);
                    }
                }
#pragma warning disable CS8603 // Possibile restituzione di riferimento Null.
                return weatherInfo;
#pragma warning restore CS8603 // Possibile restituzione di riferimento Null.
            }
        }

        private async Task<BusinessSearchEndpoint> GetBusinessSearchEndpoint(City city,int limit = 0)
        {
            BusinessSearchEndpoint? businessSearchEndpoint = null;
            using (var client = _httpClientFactory.CreateClient("Business"))
            {
                if (limit == 0)
                {
                    Int32.TryParse(_configuration["BusinessLimit"], out limit);
                }

                // Converting Request Params to Key Value Pair.  
                List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();

                //Force double parameter to use "." separator because the endpoint doesn't accept ","
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                allIputParams.Add(new KeyValuePair<string, string>("latitude", city.Latitude.ToString(nfi)));
                allIputParams.Add(new KeyValuePair<string, string>("longitude", city.Longitude.ToString(nfi)));
                //Permette di definire il numero
                allIputParams.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
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
