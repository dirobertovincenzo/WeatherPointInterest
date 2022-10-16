using System.Xml.Serialization;

namespace WeatherPointInterest.Models
{
    /// <summary>
    /// The class <c>CityDao</c> works on the database of the available cities stored in an xml
    /// </summary>
    public class CityDAO
    {
        private static readonly City[] cities;

        static CityDAO()
        {
            cities = DeserializeObject(@"data/Cities.xml");
        }

        /// <summary>
        /// Deserialize cities stored into xml file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Return all city of database
        /// </summary>
        /// <returns>Array of City items</returns>
        public City[] GetAll()
        {
            return cities;
        }

        /// <summary>
        /// Get City from id
        /// </summary>
        /// <param name="id">Integer index of city into database</param>
        /// <returns>City searched by id</returns>
        public City Get(int id)
        {
            City[] cities = GetAll();
            return cities.Length > 0 ? cities.Where(x => x.Id == id).FirstOrDefault() : null;

        }

        /// <summary>
        /// Get City from name
        /// </summary>
        /// <param name="name">Name of city</param>
        /// <returns>City searched by name</returns>
        public City Get(string name)
        {
            City[] cities = GetAll();
            return cities.Length > 0 ? cities.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault() : null;

        }

    }
}
