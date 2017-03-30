using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Providers
{
    public class GeosearchProvider : WikiProvider
    {
        public string GetUrl( string coordinates , int radius , int limit = 50 , string format = "json" )
        {
            var parameters = new[ ] {
                "action=query",
                "list=geosearch",
                $"gsradius={radius}",
                $"gscoord={coordinates}",
                $"gslimit={limit}",
                $"format={format}"
            };

            return string.Join( "&" , parameters );
        }


        public void GetArticles( Location location , int radius )
        {
            var coordinates = LocationProvider.GetCoordinates( location );
            var url = GetUrl( coordinates , radius );

            var data = ExecuteRequest( url );
            var pageIdList = GetPageIdList( data );
        }

        public List<string> GetPageIdList( string jsonData )
        {
            var result = new List<string>( );
            var jObject = JObject.Parse( jsonData );
            var query = jObject.GetValue( "query" );
            var geosearch = query[ "geosearch" ];

            foreach( var page in geosearch )
            {
                var pageId = page.Value<string>( "pageid" );
                result.Add( pageId );
            }

            return result;
        }
    }
}
