using System.Xml.Serialization;
using static WeatherPointInterest.Models.BusinessSearchEndpoint;

namespace WeatherPointInterest.Models
{
    /// <summary>
    /// The <c>City</c> class manage base info stored into xml file. It is a serializable class
    /// </summary>
    [Serializable, XmlRoot("city")]
    public class City
    {
        private int id;
        private string name;
        private double _latitude;
        private double _longitude;

#pragma warning disable CS8618 // Il campo non nullable deve contenere un valore non Null all'uscita dal costruttore. Provare a dichiararlo come nullable.
        public City(int id,string name, double latitude, double longitude)
#pragma warning restore CS8618 // Il campo non nullable deve contenere un valore non Null all'uscita dal costruttore. Provare a dichiararlo come nullable.
        {
            Id = id;
            Name = name;
            Latitude = latitude;
            Longitude = longitude;           
        }

        public City()
        {

        }

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
