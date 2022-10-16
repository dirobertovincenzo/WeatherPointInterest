using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using WeatherPointInterest.Models;

namespace WeatherPointInterest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
            
        /***
         * Return all city by Query 
         */
        public async Task<ActionResult<City[]>> GetAllCity()
        {
            City[] cities = new CityDAO().GetAll();
            return cities.Length > 0 ? cities : NotFound();
        }

        [Route("{name:alpha}")]
        public async Task<ActionResult<City>> GetCity(string name)
        {
            City city = (new CityDAO()).Get(name);
            if (city == null)
            {
                return NotFound();
            }
            return city;
        }

        [Route("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {            
            City city = (new CityDAO()).Get(id);
            if (city == null)
            {
                return NotFound();
            }
            return city;
        }


    }
}