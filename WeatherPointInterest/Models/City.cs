using System.Xml.Serialization;
using static WeatherPointInterest.Models.BusinessSearchEndpoint;

namespace WeatherPointInterest.Models
{
    [Serializable, XmlRoot("city")]
    public class City
    {
        private int id;
        private string name;
        private double _latitude;
        private double _longitude;

        [XmlElement("name")]
        public string Name { get => name; set => name = value; }
        [XmlElement("latitude")]
        public double Latitude { get => _latitude; set => _latitude = value; }
        [XmlElement("longitude")]
        public double Longitude { get => _longitude; set => _longitude = value; }
        [XmlElement("id")]
        public int Id { get => id; set => id = value; }
    }
}
