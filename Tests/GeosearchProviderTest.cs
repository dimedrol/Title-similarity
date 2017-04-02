﻿using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class GeosearchProviderTest
    {
        [TestMethod]
        public void ExecuteRequest( )
        {
            var provider = new GeosearchProvider( );
            var titles = provider.GetImageTitles( Location.Espoo , 10000 );

            var list = new List<Tuple<string , double>>( );
            var dict = new Dictionary<string , double>( );
            for( var i = 0; i < titles.Count; i++ )
            {
                var originalString = titles[ i ];
                var mostSimilarString = string.Empty;
                var maxSimilarity = (double) -1;
                for( var j = 0; j < titles.Count; j++ )
                {
                    if( i == j )
                        continue;


                    var tempSimilarity = originalString.CalculateSimilarity( titles[ j ] );

                    maxSimilarity += tempSimilarity;
                }

                list.Add( new Tuple<string , double>( originalString , maxSimilarity ) );
            }

            list = list.OrderByDescending( item => item.Item2 ).ToList( );

            list.ForEach( item =>
            {
                if( !dict.ContainsKey( item.Item1 ) )
                    dict.Add( item.Item1 , item.Item2 / list.Count );
            } );

            var resultList = dict.OrderByDescending( title => title.Value ).ToList( );
        }
    }
}
