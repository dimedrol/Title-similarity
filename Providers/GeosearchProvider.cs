using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace Providers
{
    public class GeosearchProvider : WikiProvider
    {

        public string ComposeGeosearchUrl( string coordinates , int radius , int limit = 50 , string format = "json" )
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

        public string ComposeImageTitlesUrl( string pageid , string format = "json" )
        {
            var parameters = new[ ] {
                "action=query",
                "prop=images",
               $"imlimit=max",
               $"pageids={pageid}",
               $"format={format}"
            };

            return string.Join( "&" , parameters );
        }


        public List<string> GetImageTitles( Location location , int radius )
        {
            var coordinates = LocationProvider.GetCoordinates( location );
            var url = ComposeGeosearchUrl( coordinates , radius );

            var data = ExecuteRequest( url );
            var pageIdList = GetPageIdListFromJSON( data );

            var imageTitles = GetImageTitles( pageIdList );
            return imageTitles;
        }

        public List<string> GetImageTitles( List<string> pageIdList )
        {
            var imgTitleList = new BlockingCollection<string>( );
            Parallel.ForEach( pageIdList , pageId =>
            {
                var pageImageTitles = GetImageTitlesByPageId( pageId );

                if( pageImageTitles.Any( ) )
                    pageImageTitles.ForEach( t => imgTitleList.TryAdd( t ) );
            } );

            return imgTitleList.ToList( );
        }

        public List<string> GetImageTitlesByPageId( string pageId )
        {
            var url = ComposeImageTitlesUrl( pageId );
            var data = ExecuteRequest( url );

            var imgTitleList = GetImageTitlesFromJSON( pageId , data );
            return imgTitleList;
        }

        private List<string> GetImageTitlesFromJSON( string pageId , string jsonData )
        {
            var result = new List<string>( );
            var jObject = JObject.Parse( jsonData );

            var images = jObject[ "query" ][ "pages" ][ pageId ][ "images" ] as JArray;

            if( images == null )
                return result;

            result = images.Select( i => ProcessImageTitle( i[ "title" ].ToString( ) ) ).ToList( );
            return result;
        }

        private string ProcessImageTitle( string dirtyTitle )
        {
            var cleanedTitle = string.Empty;
            var lastDotIndex = dirtyTitle.LastIndexOf( '.' );

            if( lastDotIndex >= 0 )
                dirtyTitle = dirtyTitle.Substring( 0 , lastDotIndex );

            return dirtyTitle.Replace( "File:" , string.Empty );

        }

        public List<string> GetPageIdListFromJSON( string jsonData )
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
