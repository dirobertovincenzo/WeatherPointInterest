using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using WeatherPointInterest.Models;

namespace WeatherPointInterest.Controllers
{
    /// <summary>
    /// The Controller <c>CityController</c> manage base info of the city stored into database
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        /// <summary>
        /// Gets all city available in the database 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        
        public async Task<ActionResult<City[]>> GetAllCity()
        {
            City[] cities = await new CityDAO().GetAll();
            return cities.Length > 0 ? cities : NotFound();
        }
        /// <summary>
        /// Search a city by name
        /// </summary>
        /// <param name="name">Name of the city</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{name:alpha}")]
        public async Task<ActionResult<City>> GetCity(string name)
        {
            City city = await (new CityDAO()).Get(name);
            if (city == null)
            {
                return NotFound();
            }
            return city;
        }
        /// <summary>
        /// Search a city by id
        /// </summary>
        /// <param name="id">Integer id of the city</param>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {            
            City city = await (new CityDAO()).Get(id);
            if (city == null)
            {
                return NotFound();
            }
            return city;
        }


    }
}