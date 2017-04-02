using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers
{
    public class SimilarityProvider
    {
        public static List<KeyValuePair<string , double>> ProcessStrings( List<string> stringList )
        {

            var list = new List<Tuple<string , double>>( );
            var dict = new Dictionary<string , double>( );
            for( var i = 0; i < stringList.Count; i++ )
            {
                var originalString = stringList[ i ];
                var mostSimilarString = string.Empty;
                var maxSimilarity = (double) -1;
                for( var j = 0; j < stringList.Count; j++ )
                {
                    if( i == j )
                        continue;


                    var tempSimilarity = originalString.CalculateSimilarity( stringList[ j ] );

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

            return resultList;
        }
    }
}
