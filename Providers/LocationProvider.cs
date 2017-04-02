using Data;
using System.Collections.Generic;
using System.Linq;

namespace Providers
{
    public class LocationProvider
    {
        private static Location _DefaultLocation = Location.Espoo;

        public static Dictionary<Location , string> CoordinatesDictionary = new Dictionary<Location , string>
        {
            { Location.StPetersburg,   "59.9171483|30.0448854"},
            { Location.Moscow,         "55.7494733|37.3523252" },
            { Location.NewYork,        "40.705565|-74.1180858" },
            { Location.Berlin,         "52.5074592|13.2860645" },
            { Location.Helsinki,       "60.1639305|24.9001877" },
            { Location.Espoo,          "60.2079396|24.5350342" },
            { Location.Stockholm,      "59.3260668|17.8474654" },
        };

        public static string GetCoordinates( Location location )
        {
            if( CoordinatesDictionary.ContainsKey( location ) )
                return CoordinatesDictionary[ location ];

            return CoordinatesDictionary[ _DefaultLocation ];
        }

        public static List<string> GetLocations( )
        {
            return CoordinatesDictionary.Keys.OrderBy( key => key ).Select( key => key.ToString( ) ).ToList( );
        }
    }
}
