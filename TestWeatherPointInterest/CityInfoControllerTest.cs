

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq.Protected;
using Moq;
using System.Net.Http.Headers;
using WeatherPointInterest.Controllers;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace TestWeatherPointInterest
{
    public class CityInfoControllerTest
    {
        private IConfiguration _config;

        /// <summary>
        /// Simulates the IConfiguration object used by the CityInfoController controller to read the
        /// application configuration parameters
        /// </summary>
        private IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    var builder = new ConfigurationBuilder().AddJsonFile($"testsettings.json", optional: false);
                    _config = builder.Build();
                }

                return _config;
            }
        }

#pragma warning disable CS8618 // Il campo non nullable deve contenere un valore non Null all'uscita dal costruttore. Provare a dichiararlo come nullable.
        public CityInfoControllerTest()
#pragma warning restore CS8618 // Il campo non nullable deve contenere un valore non Null all'uscita dal costruttore. Provare a dichiararlo come nullable.
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(Configuration);
        }
        /// <summary>
        /// Simulates the IHttpFactory class required by the <c>CityInfoController</c> controller for connections to the APIs 
        /// for reading weather info and the various business end points
        /// </summary>
        /// <param name="setupName"></param>
        /// <returns></returns>
        private IHttpClientFactory GetHttpClientFactory(string setupName)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
           
            HttpResponseMessage result = new HttpResponseMessage(
                HttpStatusCode.OK
            );
           
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(result)
                .Verifiable();

            //Create a Mock element that simulate IHttpClientFactory
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            switch (setupName) {
                case "Weather":
                    //Simulate HttpClient that call the Api about Weather and return an example of Json response
                    var clientWeather = new HttpClient(handlerMock.Object);
                    clientWeather.BaseAddress = new Uri(Configuration.GetValue<string>("WeatherBaseUrl"));
                    clientWeather.DefaultRequestHeaders.Clear();
                    clientWeather.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string testContent = @"{
  'cod': '200',
  'message': 0,
  'cnt': 40,
  'list': [
    {
      'dt': 1647367200,
      'main': {
        'temp': 285.44,
        'feels_like': 284.6,
        'temp_min': 284.47,
        'temp_max': 285.44,
        'pressure': 1020,
        'sea_level': 1020,
        'grnd_level': 1016,
        'humidity': 72,
        'temp_kf': 0.97
      },
      'weather': [
        {
          'id': 804,
          'main': 'Clouds',
          'description': 'overcast clouds',
          'icon': '04d'
        }
      ],
      'clouds': {
        'all': 90
      },
      'wind': {
        'speed': 2.7,
        'deg': 183,
        'gust': 5.59
      },
      'visibility': 10000,
      'pop': 0,
      'sys': {
        'pod': 'd'
      },
      'dt_txt': '2022-03-15 18:00:00'
    }
  ],
  'city': {
    'id': 1,
    'name': 'Napoli',
    'coord': {
      'lat': 40.8358846,
      'lon': 14.2487679
    },
  }
}";
                    result.Content = new StringContent(testContent);

                    mockHttpClientFactory.Setup(_ => _.CreateClient("Weather")).Returns(clientWeather);

                    break;
                case "Business":
                    //Simulate HttpClient that call the Api about BusinessEndPoints and return an example of Json response
                    var clientBusiness = new HttpClient(handlerMock.Object);
                    clientBusiness.BaseAddress = new Uri(Configuration.GetValue<string>("BusinessBaseUrl"));
                    clientBusiness.DefaultRequestHeaders.Clear();
                    clientBusiness.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientBusiness.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configuration.GetValue<string>("BusinessAPIKey"));
                    string content = "{\"businesses\": [{\"id\": \"cyax2FRp8e4Q2MlaH5L5ew\", \"alias\": \"l-antica-pizzeria-da-michele-napoli-2\", \"name\": \"L'Antica Pizzeria Da Michele\", \"image_url\": \"https://s3-media1.fl.yelpcdn.com/bphoto/1YSqzIs_k5UdNNfq9-dVsQ/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/l-antica-pizzeria-da-michele-napoli-2?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 644, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.84978, \"longitude\": 14.26322}, \"transactions\": [], \"price\": \"\\u20ac\", \"location\": {\"address1\": \"Via Cesare Sersale 1\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80139\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Cesare Sersale 1\", \"80139 Naples\", \"Italy\"]}, \"phone\": \"+390815539204\", \"display_phone\": \"+39 081 553 9204\", \"distance\": 1966.4970246669811}, {\"id\": \"NTMxtFdlQ8dU8sRPOagVhA\", \"alias\": \"sorbillo-napoli-4\", \"name\": \"Sorbillo\", \"image_url\": \"https://s3-media1.fl.yelpcdn.com/bphoto/zjlwlAlHDBpUcKezjIDrMA/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/sorbillo-napoli-4?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 351, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.8504755, \"longitude\": 14.2553663}, \"transactions\": [], \"price\": \"\\u20ac\", \"location\": {\"address1\": \"Via dei Tribunali 32\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80138\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via dei Tribunali 32\", \"80138 Naples\", \"Italy\"]}, \"phone\": \"+39081446643\", \"display_phone\": \"+39 081 446643\", \"distance\": 1707.8463731582242}, {\"id\": \"uPWfFoxQWFl-1UaFYc9nTQ\", \"alias\": \"tandem-napoli\", \"name\": \"Tandem\", \"image_url\": \"https://s3-media3.fl.yelpcdn.com/bphoto/a1lMSIrl-8K2jJMgHbvoHg/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/tandem-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 144, \"categories\": [{\"alias\": \"italian\", \"title\": \"Italian\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.8482797, \"longitude\": 14.2562832}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Paladino 51\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80138\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Paladino 51\", \"80138 Naples\", \"Italy\"]}, \"phone\": \"+3908119002468\", \"display_phone\": \"+39 081 1900 2468\", \"distance\": 1516.3428410379129}, {\"id\": \"MtdrmfmtsyWseyX4i-Wz1A\", \"alias\": \"il-gobbetto-napoli\", \"name\": \"Il Gobbetto\", \"image_url\": \"https://s3-media1.fl.yelpcdn.com/bphoto/klY1fx4DJ-Z6riy1nZok9w/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/il-gobbetto-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 60, \"categories\": [{\"alias\": \"seafood\", \"title\": \"Seafood\"}, {\"alias\": \"napoletana\", \"title\": \"Napoletana\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.8379707336426, \"longitude\": 14.2478857040405}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Sergente Maggiore 8\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80132\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Sergente Maggiore 8\", \"80132 Naples\", \"Italy\"]}, \"phone\": \"+390812512435\", \"display_phone\": \"+39 081 251 2435\", \"distance\": 242.36295455328943}, {\"id\": \"gntECd92liRI0mL0fVP3wQ\", \"alias\": \"gran-caff\\u00e8-gambrinus-napoli-3\", \"name\": \"Gran Caff\\u00e8 Gambrinus\", \"image_url\": \"https://s3-media4.fl.yelpcdn.com/bphoto/Z2_wKXVxT1_ZkpZj44wCdQ/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/gran-caff%C3%A8-gambrinus-napoli-3?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 121, \"categories\": [{\"alias\": \"desserts\", \"title\": \"Desserts\"}, {\"alias\": \"cafes\", \"title\": \"Cafes\"}], \"rating\": 3.5, \"coordinates\": {\"latitude\": 40.8369065686041, \"longitude\": 14.2485313117504}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Chiaia 1\", \"address2\": null, \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80121\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Chiaia 1\", \"80121 Naples\", \"Italy\"]}, \"phone\": \"+39081417582\", \"display_phone\": \"+39 081 417582\", \"distance\": 115.3674928032443}, {\"id\": \"oUELqLNyUvZ3byW3-mfIrA\", \"alias\": \"di-matteo-napoli\", \"name\": \"Di Matteo\", \"image_url\": \"https://s3-media4.fl.yelpcdn.com/bphoto/0Jv8tqjzAjj3GATWKWXr9g/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/di-matteo-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 173, \"categories\": [{\"alias\": \"italian\", \"title\": \"Italian\"}, {\"alias\": \"pizza\", \"title\": \"Pizza\"}], \"rating\": 4.0, \"coordinates\": {\"latitude\": 40.8512355, \"longitude\": 14.2580897}, \"transactions\": [], \"price\": \"\\u20ac\", \"location\": {\"address1\": \"Via Tribunali 94\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80138\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Tribunali 94\", \"80138 Naples\", \"Italy\"]}, \"phone\": \"+39081455262\", \"display_phone\": \"+39 081 455262\", \"distance\": 1878.434572620036}, {\"id\": \"3-vO15CGe4XPEHx_8yWPLA\", \"alias\": \"pizzeria-napoli-in-bocca-napoli\", \"name\": \"Pizzeria Napoli in Bocca\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/JZMPm-obefl5cx6mZ0GI-w/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/pizzeria-napoli-in-bocca-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 43, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.8379483469731, \"longitude\": 14.2498201131821}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via San Carlo 15\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80133\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via San Carlo 15\", \"80133 Naples\", \"Italy\"]}, \"phone\": \"+39081426300\", \"display_phone\": \"+39 081 426300\", \"distance\": 245.95903902655076}, {\"id\": \"9pG0qc9203ouMV0WuAR1EQ\", \"alias\": \"librerie-feltrinelli-napoli-2\", \"name\": \"Librerie Feltrinelli\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/DtK_0fgwSBG4L_aRSwDMtg/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/librerie-feltrinelli-napoli-2?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 27, \"categories\": [{\"alias\": \"bookstores\", \"title\": \"Bookstores\"}, {\"alias\": \"cafeteria\", \"title\": \"Cafeteria\"}, {\"alias\": \"musicvideo\", \"title\": \"Music & DVDs\"}], \"rating\": 5.0, \"coordinates\": {\"latitude\": 40.83486, \"longitude\": 14.24224}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Santa Caterina 23\", \"address2\": null, \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80121\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Santa Caterina 23\", \"80121 Naples\", \"Italy\"]}, \"phone\": \"+390812405411\", \"display_phone\": \"+39 081 240 5411\", \"distance\": 558.5049397871804}, {\"id\": \"8bK2N6l36vxVyW6_RARxQQ\", \"alias\": \"birdys-bakery-no-title\", \"name\": \"Birdy's Bakery\", \"image_url\": \"https://s3-media1.fl.yelpcdn.com/bphoto/fCn0NpwYF4bAlgd7RJNsoA/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/birdys-bakery-no-title?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 73, \"categories\": [{\"alias\": \"breakfast_brunch\", \"title\": \"Breakfast & Brunch\"}, {\"alias\": \"bakeries\", \"title\": \"Bakeries\"}, {\"alias\": \"cafes\", \"title\": \"Cafes\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.8353347270466, \"longitude\": 14.2386485229196}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Vico Belledonne a Chiaia 14b\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Napoli\", \"zip_code\": \"\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Vico Belledonne a Chiaia 14b\", \"Napoli\", \"Italy\"]}, \"phone\": \"+390814976400\", \"display_phone\": \"+39 081 497 6400\", \"distance\": 853.5233146745186}, {\"id\": \"xczybYFlz7NfRNY1zQfW_g\", \"alias\": \"pizzeria-da-attilio-napoli\", \"name\": \"Pizzeria Da Attilio\", \"image_url\": \"https://s3-media4.fl.yelpcdn.com/bphoto/KXvziAZv7Qhocw4sr5NQXA/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/pizzeria-da-attilio-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 48, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}, {\"alias\": \"friterie\", \"title\": \"Friterie\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.845301868655, \"longitude\": 14.2483051166062}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Pignasecca 17\", \"address2\": null, \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80134\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Pignasecca 17\", \"80134 Naples\", \"Italy\"]}, \"phone\": \"+390815520479\", \"display_phone\": \"+39 081 552 0479\", \"distance\": 1047.874754365159}, {\"id\": \"8eCzJP5GQQrC_oM46NWwnA\", \"alias\": \"piazza-del-plebiscito-napoli\", \"name\": \"Piazza del Plebiscito\", \"image_url\": \"https://s3-media1.fl.yelpcdn.com/bphoto/Tyoy1LvbCTNBAjIVcaWqAA/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/piazza-del-plebiscito-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 18, \"categories\": [{\"alias\": \"landmarks\", \"title\": \"Landmarks & Historical Buildings\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.8360901, \"longitude\": 14.2491598}, \"transactions\": [], \"location\": {\"address1\": \"Piazza del Plebiscito\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Piazza del Plebiscito\", \"Naples, Napoli\", \"Italy\"]}, \"phone\": \"\", \"display_phone\": \"\", \"distance\": 86.006226713625}, {\"id\": \"4mTLnVVOy9hCISc3Hpw3KA\", \"alias\": \"evento-elite-yelp-drinks-aperistreet-al-pdm-napoli\", \"name\": \"Evento Elite Yelp Drinks Aperistreet al PdM\", \"image_url\": \"https://s3-media1.fl.yelpcdn.com/bphoto/3qYZ64ANO4xBQ3vH2U8s1g/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/evento-elite-yelp-drinks-aperistreet-al-pdm-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 17, \"categories\": [{\"alias\": \"yelpevents\", \"title\": \"Yelp Events\"}], \"rating\": 5.0, \"coordinates\": {\"latitude\": 40.834322, \"longitude\": 14.241822}, \"transactions\": [], \"location\": {\"address1\": \"Piazza dei Martiri\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80121\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Piazza dei Martiri\", \"80121 Naples\", \"Italy\"]}, \"phone\": \"\", \"display_phone\": \"\", \"distance\": 619.4414553834994}, {\"id\": \"Vb692I0fylZlNwinsXuthA\", \"alias\": \"trattoria-da-nennella-napoli\", \"name\": \"Trattoria da Nennella\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/4G3S4zVYOsvnlus5f3WQ5g/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/trattoria-da-nennella-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 81, \"categories\": [{\"alias\": \"trattorie\", \"title\": \"Trattorie\"}, {\"alias\": \"napoletana\", \"title\": \"Napoletana\"}, {\"alias\": \"seafood\", \"title\": \"Seafood\"}], \"rating\": 4.0, \"coordinates\": {\"latitude\": 40.842002896426, \"longitude\": 14.2475810095321}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Lungo Teatro Nuovo 105\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80134\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Lungo Teatro Nuovo 105\", \"80134 Naples\", \"Italy\"]}, \"phone\": \"+39081414338\", \"display_phone\": \"+39 081 414338\", \"distance\": 687.6106515507513}, {\"id\": \"BSiWzaCF9h11NiFrtR3tBw\", \"alias\": \"starita-napoli-2\", \"name\": \"Starita\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/fFBBdEXXyaDuMWz17kcDmw/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/starita-napoli-2?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 105, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}, {\"alias\": \"gluten_free\", \"title\": \"Gluten-Free\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.8559399, \"longitude\": 14.24646}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Materdei 27\", \"address2\": null, \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80136\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Materdei 27\", \"80136 Naples\", \"Italy\"]}, \"phone\": \"+390815573682\", \"display_phone\": \"+39 081 557 3682\", \"distance\": 2218.8593920732937}, {\"id\": \"lchcDkoEptFp_68fHlOqSw\", \"alias\": \"il-vero-bar-del-professore-napoli-2\", \"name\": \"Il Vero Bar del Professore\", \"image_url\": \"https://s3-media4.fl.yelpcdn.com/bphoto/2HcKaficnimmbT0WiPuVag/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/il-vero-bar-del-professore-napoli-2?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 50, \"categories\": [{\"alias\": \"cafes\", \"title\": \"Cafes\"}], \"rating\": 4.0, \"coordinates\": {\"latitude\": 40.837063, \"longitude\": 14.248472}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Piazza Trieste e Trento 46\", \"address2\": \"\", \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80132\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Piazza Trieste e Trento 46\", \"80132 Naples\", \"Italy\"]}, \"phone\": \"+39081403041\", \"display_phone\": \"+39 081 403041\", \"distance\": 133.3756104253289}, {\"id\": \"FMJBmQBACM9L8xI1KUg4-A\", \"alias\": \"galleria-borbonica-napoli-2\", \"name\": \"Galleria Borbonica\", \"image_url\": \"https://s3-media3.fl.yelpcdn.com/bphoto/Y8RKglxMHOwwwQeXGxeV1Q/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/galleria-borbonica-napoli-2?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 18, \"categories\": [{\"alias\": \"museums\", \"title\": \"Museums\"}, {\"alias\": \"landmarks\", \"title\": \"Landmarks & Historical Buildings\"}], \"rating\": 5.0, \"coordinates\": {\"latitude\": 40.8328633206978, \"longitude\": 14.2432832299018}, \"transactions\": [], \"location\": {\"address1\": \"Vico del Grottone 4\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80132\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Vico del Grottone 4\", \"80132 Naples\", \"Italy\"]}, \"phone\": \"+390817645808\", \"display_phone\": \"+39 081 764 5808\", \"distance\": 570.7693550102026}, {\"id\": \"c5j7xFeaaufdoL5lDbBrkg\", \"alias\": \"pizzeria-trianon-napoli\", \"name\": \"Pizzeria Trianon\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/Q_9MO9mxwEEylxEz8ymaNw/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/pizzeria-trianon-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 95, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.85111, \"longitude\": 14.26361}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Pietro Colletta 44\", \"address2\": null, \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80139\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Pietro Colletta 44\", \"80139 Naples\", \"Italy\"]}, \"phone\": \"+390815539426\", \"display_phone\": \"+39 081 553 9426\", \"distance\": 2021.4494155974958}, {\"id\": \"VAXS_6WCc_H1wHSV7WucNA\", \"alias\": \"casa-infante-napoli-4\", \"name\": \"Casa Infante\", \"image_url\": \"https://s3-media1.fl.yelpcdn.com/bphoto/CsNDJ6phv4JZX_WNpZ_D9g/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/casa-infante-napoli-4?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 34, \"categories\": [{\"alias\": \"icecream\", \"title\": \"Ice Cream & Frozen Yogurt\"}, {\"alias\": \"gelato\", \"title\": \"Gelato\"}], \"rating\": 4.0, \"coordinates\": {\"latitude\": 40.8386688, \"longitude\": 14.2482653}, \"transactions\": [], \"price\": \"\\u20ac\", \"location\": {\"address1\": \"Via Toledo 258\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80132\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Toledo 258\", \"80132 Naples\", \"Italy\"]}, \"phone\": \"+3908119312009\", \"display_phone\": \"+39 081 1931 2009\", \"distance\": 313.4301754333459}, {\"id\": \"A8IiTHxgyajja02-1wXwaw\", \"alias\": \"pastamore-e-chiatamone-napoli\", \"name\": \"Pastamore & Chiatamone\", \"image_url\": \"https://s3-media4.fl.yelpcdn.com/bphoto/gN46rCYNzVIGVfLtMEFPsw/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/pastamore-e-chiatamone-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 39, \"categories\": [{\"alias\": \"napoletana\", \"title\": \"Napoletana\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.83047429, \"longitude\": 14.24555765}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Chiatamone 56\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80121\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Chiatamone 56\", \"80121 Naples\", \"Italy\"]}, \"phone\": \"+390810320313\", \"display_phone\": \"+39 081 032 0313\", \"distance\": 653.1896887051561}, {\"id\": \"hxK8HweUPetsFJf8O70skA\", \"alias\": \"maschio-angioino-castel-nuovo-napoli\", \"name\": \"Maschio Angioino - Castel Nuovo\", \"image_url\": \"https://s3-media3.fl.yelpcdn.com/bphoto/FrQo___NYE4O6IywAKeE_A/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/maschio-angioino-castel-nuovo-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 22, \"categories\": [{\"alias\": \"landmarks\", \"title\": \"Landmarks & Historical Buildings\"}, {\"alias\": \"localflavor\", \"title\": \"Local Flavor\"}, {\"alias\": \"castles\", \"title\": \"Castles\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.838504, \"longitude\": 14.252675}, \"transactions\": [], \"location\": {\"address1\": \"Piazza Municipio\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80133\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Piazza Municipio\", \"80133 Naples\", \"Italy\"]}, \"phone\": \"+390817957713\", \"display_phone\": \"+39 081 795 7713\", \"distance\": 439.17270308239387}, {\"id\": \"jryPrb0e4cT2udJM4H7zHw\", \"alias\": \"iki-napoli-2\", \"name\": \"Iki\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/RIklLCwlkXD3VjS6Mh8-8g/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/iki-napoli-2?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 30, \"categories\": [{\"alias\": \"japanese\", \"title\": \"Japanese\"}, {\"alias\": \"modern_european\", \"title\": \"Modern European\"}, {\"alias\": \"asianfusion\", \"title\": \"Asian Fusion\"}], \"rating\": 4.0, \"coordinates\": {\"latitude\": 40.8372003076248, \"longitude\": 14.2473166063428}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Nardones 103\", \"address2\": null, \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80132\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Nardones 103\", \"80132 Naples\", \"Italy\"]}, \"phone\": \"+3908118639120\", \"display_phone\": \"+39 081 1863 9120\", \"distance\": 190.55324903295937}, {\"id\": \"v-AjqZQGqLNrs0cy8VjNJA\", \"alias\": \"antica-pizzeria-prigiobbo-napoli\", \"name\": \"Antica Pizzeria Prigiobbo\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/eDP8ciEE6eCOZFcmY1_W1g/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/antica-pizzeria-prigiobbo-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 26, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}, {\"alias\": \"italian\", \"title\": \"Italian\"}, {\"alias\": \"mediterranean\", \"title\": \"Mediterranean\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.8417132428617, \"longitude\": 14.2479565104508}, \"transactions\": [], \"price\": \"\\u20ac\", \"location\": {\"address1\": \"Via Portacarrese A Montecalvario 96\", \"address2\": null, \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80134\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Portacarrese A Montecalvario 96\", \"80134 Naples\", \"Italy\"]}, \"phone\": \"+39081407692\", \"display_phone\": \"+39 081 407692\", \"distance\": 651.6992637553922}, {\"id\": \"tHOAoIX04BlA6FbufEmJlg\", \"alias\": \"napoli-sotterranea-napoli-2\", \"name\": \"Napoli Sotterranea\", \"image_url\": \"https://s3-media3.fl.yelpcdn.com/bphoto/5b4giQGYvbJ0Byln27pdYg/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/napoli-sotterranea-napoli-2?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 44, \"categories\": [{\"alias\": \"landmarks\", \"title\": \"Landmarks & Historical Buildings\"}, {\"alias\": \"historicaltours\", \"title\": \"Historical Tours\"}, {\"alias\": \"walkingtours\", \"title\": \"Walking Tours\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.8511433187946, \"longitude\": 14.2567747074501}, \"transactions\": [], \"location\": {\"address1\": \"Piazza San Gaetano 68\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80138\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Piazza San Gaetano 68\", \"80138 Naples\", \"Italy\"]}, \"phone\": \"+39081296944\", \"display_phone\": \"+39 081 296944\", \"distance\": 1825.4835175234787}, {\"id\": \"jbPRx9dY0_F49wC68Y3Ivg\", \"alias\": \"50-kal\\u00f2-napoli-2\", \"name\": \"50 Kal\\u00f2\", \"image_url\": \"https://s3-media3.fl.yelpcdn.com/bphoto/V4by0cs67Vdnb9oAYLO5kg/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/50-kal%C3%B2-napoli-2?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 108, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}], \"rating\": 4.0, \"coordinates\": {\"latitude\": 40.8286636129902, \"longitude\": 14.2200089920194}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Piazza Sannazzaro 201B\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80122\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Piazza Sannazzaro 201B\", \"80122 Naples\", \"Italy\"]}, \"phone\": \"+3908119204667\", \"display_phone\": \"+39 081 1920 4667\", \"distance\": 2549.321022697946}, {\"id\": \"bCIM_wAk86odc5OfSrvkQQ\", \"alias\": \"locanda-del-cerriglio-napoli\", \"name\": \"Locanda del Cerriglio\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/qJATSQfUCpOUDRrL4JEpkQ/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/locanda-del-cerriglio-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 39, \"categories\": [{\"alias\": \"napoletana\", \"title\": \"Napoletana\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.843853, \"longitude\": 14.2541456}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via del Cerriglio 3\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80134\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via del Cerriglio 3\", \"80134 Naples\", \"Italy\"]}, \"phone\": \"+390815526406\", \"display_phone\": \"+39 081 552 6406\", \"distance\": 994.7221342776738}, {\"id\": \"T5RR7uhSFuHkQQM_DKleCA\", \"alias\": \"antico-forno-attanasio-napoli\", \"name\": \"Antico Forno Attanasio\", \"image_url\": \"https://s3-media3.fl.yelpcdn.com/bphoto/Zz5u_bb4M6B3DHcXxwiiTw/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/antico-forno-attanasio-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 44, \"categories\": [{\"alias\": \"chocolate\", \"title\": \"Chocolatiers & Shops\"}, {\"alias\": \"desserts\", \"title\": \"Desserts\"}, {\"alias\": \"cakeshop\", \"title\": \"Patisserie/Cake Shop\"}], \"rating\": 5.0, \"coordinates\": {\"latitude\": 40.8531276163569, \"longitude\": 14.269060226555}, \"transactions\": [], \"price\": \"\\u20ac\", \"location\": {\"address1\": \"Vico Ferrovia 1\", \"address2\": null, \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80142\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Vico Ferrovia 1\", \"80142 Naples\", \"Italy\"]}, \"phone\": \"+39081285675\", \"display_phone\": \"+39 081 285675\", \"distance\": 2567.0623265765926}, {\"id\": \"Afznq65VAbAL6uXjjll7hQ\", \"alias\": \"val\\u00f9-napoli\", \"name\": \"Val\\u00f9\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/rj0oaZR_j4XsAVP6LRbZ6Q/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/val%C3%B9-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 30, \"categories\": [{\"alias\": \"italian\", \"title\": \"Italian\"}, {\"alias\": \"wine_bars\", \"title\": \"Wine Bars\"}, {\"alias\": \"bbq\", \"title\": \"Barbeque\"}], \"rating\": 4.0, \"coordinates\": {\"latitude\": 40.8406791687012, \"longitude\": 14.2480459213257}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Vico Lungo del Gelso 80\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80134\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Vico Lungo del Gelso 80\", \"80134 Naples\", \"Italy\"]}, \"phone\": \"+390810381139\", \"display_phone\": \"+39 081 038 1139\", \"distance\": 547.2004421456097}, {\"id\": \"5be2vuy9S5vbzIAz9wwSSg\", \"alias\": \"pizzeria-brandi-napoli\", \"name\": \"Pizzeria Brandi\", \"image_url\": \"https://s3-media1.fl.yelpcdn.com/bphoto/E2aephGKq3O1dXuFQO-bgg/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/pizzeria-brandi-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 67, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}, {\"alias\": \"cucinacampana\", \"title\": \"Cucina campana\"}], \"rating\": 3.5, \"coordinates\": {\"latitude\": 40.83664451, \"longitude\": 14.24686547}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Salita Sant' Anna di Palazzo 1\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80132\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Salita Sant' Anna di Palazzo 1\", \"80132 Naples\", \"Italy\"]}, \"phone\": \"+39081416928\", \"display_phone\": \"+39 081 416928\", \"distance\": 180.98372865597912}, {\"id\": \"szzBPMZAF-_Jqsqp9VaeOA\", \"alias\": \"antonio-e-antonio-napoli-3\", \"name\": \"Antonio & Antonio\", \"image_url\": \"https://s3-media3.fl.yelpcdn.com/bphoto/ytGam2t7uO8NcsiKjiC-cw/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/antonio-e-antonio-napoli-3?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 58, \"categories\": [{\"alias\": \"pizza\", \"title\": \"Pizza\"}, {\"alias\": \"italian\", \"title\": \"Italian\"}], \"rating\": 4.0, \"coordinates\": {\"latitude\": 40.83002, \"longitude\": 14.24605}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Partenope 24\", \"address2\": null, \"address3\": null, \"city\": \"Naples\", \"zip_code\": \"80121\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Partenope 24\", \"80121 Naples\", \"Italy\"]}, \"phone\": \"+390812451987\", \"display_phone\": \"+39 081 245 1987\", \"distance\": 696.7613168949632}, {\"id\": \"N6l9JAGlTqza1r59aOWa9Q\", \"alias\": \"antica-capri-napoli\", \"name\": \"Antica Capri\", \"image_url\": \"https://s3-media2.fl.yelpcdn.com/bphoto/Hl-e1SDW1_CMvHP1KpCJWw/o.jpg\", \"is_closed\": false, \"url\": \"https://www.yelp.com/biz/antica-capri-napoli?adjust_creative=3r8NSYNdRscKS90dybznOQ&utm_campaign=yelp_api_v3&utm_medium=api_v3_business_search&utm_source=3r8NSYNdRscKS90dybznOQ\", \"review_count\": 28, \"categories\": [{\"alias\": \"trattorie\", \"title\": \"Trattorie\"}, {\"alias\": \"pizza\", \"title\": \"Pizza\"}, {\"alias\": \"seafood\", \"title\": \"Seafood\"}], \"rating\": 4.5, \"coordinates\": {\"latitude\": 40.83956, \"longitude\": 14.24758}, \"transactions\": [], \"price\": \"\\u20ac\\u20ac\", \"location\": {\"address1\": \"Via Speranzella 110\", \"address2\": \"\", \"address3\": \"\", \"city\": \"Naples\", \"zip_code\": \"80132\", \"country\": \"IT\", \"state\": \"NA\", \"display_address\": [\"Via Speranzella 110\", \"80132 Naples\", \"Italy\"]}, \"phone\": \"+390810383486\", \"display_phone\": \"+39 081 038 3486\", \"distance\": 361.58528922990513}], \"total\": 2000, \"region\": {\"center\": {\"longitude\": 14.2487679, \"latitude\": 40.8358846}}}";

                    result.Content = new StringContent(content);
                    mockHttpClientFactory.Setup(_ => _.CreateClient("Business")).Returns(clientBusiness);

                    break;
            }

            return mockHttpClientFactory.Object;

        }



        /// <summary>
        /// Call the <c>GetWeatherOfCity</c> method with two different values. One not present in the system which returns 
        /// the NotFound error and the other which instead returns the json expected by the test
        /// </summary>
        /// <param name="id">Id of the city</param>
        /// <returns></returns>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
 
        public async Task TestGetWeatherOfCity(int id)
        {
            IHttpClientFactory _httpClientFactory = GetHttpClientFactory("Weather");
            var controller = new CityInfoController(_config,_httpClientFactory);
            var result = await controller.GetWeatherOfCity(id, _config.GetValue<int>("WeatherCntDefault"));
            switch (id)
            {
                case 0:
                    Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result.Result);
                     break;
                case 1:
                    Assert.IsType<ActionResult<WeatherInfo>>(result);
                    Assert.NotNull(result);
#pragma warning disable CS8602 // Dereferenziamento di un possibile riferimento Null.
                    Assert.Equal("Napoli",result.Value.city.name);
                    Assert.Equal(40.8358846, result.Value.city.coord.lat);
                    Assert.Equal(14.2487679, result.Value.city.coord.lon);
#pragma warning restore CS8602 // Dereferenziamento di un possibile riferimento Null.
                    break;
            }
        }

        /// <summary>
        /// Call the <c>GetWeatherOfCity</c> method with two different values. One not present in the system which returns 
        /// the NotFound error and the other which instead returns the json expected by the test
        /// </summary>
        /// <param name="name">Name of the city</param>
        /// <returns></returns>
        [Theory]
        [InlineData("Napoli")]
        [InlineData("Parigi")]
        //Pass only one correct Name  
        public async Task TestGetWeatherOfCityName(string name)
        {
            IHttpClientFactory _httpClientFactory = GetHttpClientFactory("Weather");
            var controller = new CityInfoController(_config, _httpClientFactory);
            var result = await controller.GetWeatherOfCity(name, _config.GetValue<int>("WeatherCntDefault"));
            switch (name)
            {
                case "Parigi":
                    Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result.Result);
                    break;
                case "Napoli":
                    Assert.IsType<ActionResult<WeatherInfo>>(result);
                    Assert.NotNull(result);
#pragma warning disable CS8602 // Dereferenziamento di un possibile riferimento Null.
                    Assert.Equal("Napoli", result.Value.city.name);
                    Assert.Equal(40.8358846, result.Value.city.coord.lat);
                    Assert.Equal(14.2487679, result.Value.city.coord.lon);
#pragma warning restore CS8602 // Dereferenziamento di un possibile riferimento Null.
                    break;
            }
        }

        /// <summary>
        /// Call the <c>GetBusinessOfCity</c> method with two different values. One not present in the system which returns 
        /// the NotFound error and the other which instead returns the json expected by the test
        /// </summary>
        /// <param name="id">Id of the city</param>
        /// <returns></returns>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        //Pass only one correct id  
        public async Task TestGetBusinessOfCity(int id)
        {
            IHttpClientFactory _httpClientFactory = GetHttpClientFactory("Business");
            var controller = new CityInfoController(_config, _httpClientFactory);
            var result = await controller.GetBusinessOfCity(id, _config.GetValue<int>("BusinessLimit"));
            switch (id)
            {
                case 0:
                    Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result.Result);
                    break;
                case 1:
                    Assert.IsType<ActionResult<BusinessSearchEndpoint>>(result);
                    Assert.NotNull(result);
#pragma warning disable CS8602 // Dereferenziamento di un possibile riferimento Null.
                    Assert.Equal(_config.GetValue<int>("BusinessLimit"), result.Value.businesses.Count);
                    Assert.Equal(40.8358846, result.Value.region.center.latitude);
                    Assert.Equal(14.2487679, result.Value.region.center.longitude);
#pragma warning restore CS8602 // Dereferenziamento di un possibile riferimento Null.
                    break;
            }
        }


        /// <summary>
        /// Call the <c>GetBusinessOfCity</c> method with two different values. One not present in the system which returns 
        /// the NotFound error and the other which instead returns the json expected by the test
        /// </summary>
        /// <param name="name">Name of the city</param>
        /// <returns></returns>
        [Theory]
        [InlineData("Napoli")]
        [InlineData("Parigi")]
        //Pass only one correct id  
        public async Task TestGetBusinessOfCityName(string name)
        {
            IHttpClientFactory _httpClientFactory = GetHttpClientFactory("Business");
            var controller = new CityInfoController(_config, _httpClientFactory);
            var result = await controller.GetBusinessOfCity(name, _config.GetValue<int>("BusinessLimit"));
            switch (name)
            {
                case "Parigi":
                    Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result.Result);
                    break;
                case "Napoli":
                    Assert.IsType<ActionResult<BusinessSearchEndpoint>>(result);
                    Assert.NotNull(result);
#pragma warning disable CS8602 // Dereferenziamento di un possibile riferimento Null.
                    Assert.Equal(_config.GetValue<int>("BusinessLimit"), result.Value.businesses.Count);
                    Assert.Equal(40.8358846, result.Value.region.center.latitude);
                    Assert.Equal(14.2487679, result.Value.region.center.longitude);
#pragma warning restore CS8602 // Dereferenziamento di un possibile riferimento Null.
                    break;
            }
        }



    }
}