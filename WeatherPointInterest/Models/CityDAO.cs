using System.Xml.Serialization;

namespace WeatherPointInterest.Models
{
   
    public class CityDAO
    {
        private static readonly City[] cities;

        static CityDAO()
        {
            cities = DeserializeObject(@"data/Cities.xml");
        }

        private static City[] DeserializeObject(string filename)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(City[]));
            City[] cities = new City[0];
            if (File.Exists(filename))
            {
                FileStream fs = new FileStream(filename, FileMode.Open);
#pragma warning disable CS8600 // Conversione del valore letterale Null o di un possibile valore Null in un tipo che non ammette i valori Null.
                cities = mySerializer.Deserialize(fs) as City[];
#pragma warning restore CS8600 // Conversione del valore letterale Null o di un possibile valore Null in un tipo che non ammette i valori Null.
                fs.Close();
            }
            return cities;
        }


        public City[] GetAll()
        {
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
