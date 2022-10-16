

using Microsoft.AspNetCore.Mvc;
using WeatherPointInterest.Controllers;

namespace TestWeatherPointInterest
{
    public class CityControllerTest
    {
        [Fact]
        public async Task TestGetAllCity()
        {
            var controller = new CityController();
            /* This is the data value of Napoli City in data xml
             * I verify that the CityDao Model looks for the city like Naples and
             * that the parameters are correctly valued
             * <City>
		            <name>Napoli</name>
		            <latitude>40.8358846</latitude>
		            <longitude>14.2487679</longitude>
		            <id>1</id>
	           </City>             
             */
            City city = new City(
                0,
                "Napoli",
                0,
                0
            );
            CityDAO cityDAO = new CityDAO();
            var cities =  await controller.GetAllCity();
            Assert.IsType < ActionResult<City[]>>(cities);
            Assert.NotNull(cities.Value);
#pragma warning disable CS8600,CS8604 // Conversione del valore letterale Null o di un possibile valore Null in un tipo che non ammette i valori Null.
            City naples = cities.Value.Where(x => x.Name == "Napoli").FirstOrDefault();
#pragma warning restore CS8600, CS8604 // Conversione del valore letterale Null o di un possibile valore Null in un tipo che non ammette i valori Null.
            Assert.NotNull(naples);
            if (naples != null)
            {
                Assert.False(naples.Id == city.Id);
                Assert.True(naples.Id == 1);

                Assert.False(naples.Latitude == city.Latitude);
                Assert.True(naples.Latitude == 40.8358846);

                Assert.False(naples.Longitude == city.Longitude);
                Assert.True(naples.Longitude == 14.2487679);

            }
        }

        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        //I check that the system has read all the records from the data file Cities.xml
        public async Task TestGetCity(int id)
        {
            var controller = new CityController();
            var city = await controller.GetCity(id);
            Assert.IsType<ActionResult<City>>(city);
            switch (id)
            {
                case >=1 and <=5:
                    Assert.NotNull(city.Value);
                    break;
                case 0 or > 5:
                    Assert.Null(city.Value);
                    break;
                default:
                    Assert.Null(city.Value);
                    break;
            }
        }

        [Theory]
        [InlineData("Napoli")]
        [InlineData("Roma")]
        [InlineData("Milano")]
        [InlineData("Rionero in Vulture")]
        [InlineData("Torino")]
        //I check that the system has read all the records from the data file Cities.xml
        public async Task TestGetCityName(string name)
        {
            var controller = new CityController();
            var city = await controller.GetCity(name);
            Assert.IsType<ActionResult<City>>(city);          
            Assert.NotNull(city);
        }


    }
}