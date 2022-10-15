using System.Xml.Serialization;

namespace WeatherPointInterest.Models
{
    public class CityDAO
    {
        public City[] GetAll()
        {
            City[] cities = DeserializeObject(@"data/Cities.xml");
            return cities;
        }
        private City[] DeserializeObject(string filename)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(City[]));
            FileStream fs = new FileStream(filename, FileMode.Open);
            City[] cities = (City[])mySerializer.Deserialize(fs);
            fs.Close();
            return cities;
        }

        //Select city by id
        public City Get(int id)
        {
            City[] cities = GetAll();
            return cities.Length > 0 ? cities.Where(x => x.Id == id).FirstOrDefault() : null;

        }

        //Select city by name
        public City Get(string name)
        {
            City[] cities = GetAll();
            return cities.Length > 0 ? cities.Where(x => x.Name == name).FirstOrDefault() : null;

        }

    }
}
