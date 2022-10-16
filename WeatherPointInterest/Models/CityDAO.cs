using System.Xml.Serialization;

namespace WeatherPointInterest.Models
{
   
    public class CityDAO
    {
      
        public City[] GetAll()
        {
            //string path = System.IO.Path.Combine(_env.WebRootPath, "/data/Cities.xml");
            City[] cities = DeserializeObject(@"data/Cities.xml");
            return cities;
        }
        private City[] DeserializeObject(string filename)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(City[]));
            FileStream fs = new FileStream(filename, FileMode.Open);
#pragma warning disable CS8600 // Conversione del valore letterale Null o di un possibile valore Null in un tipo che non ammette i valori Null.
            City[] cities = mySerializer.Deserialize(fs) as City[];
#pragma warning restore CS8600 // Conversione del valore letterale Null o di un possibile valore Null in un tipo che non ammette i valori Null.
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
