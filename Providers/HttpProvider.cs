using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Providers
{
    public class HttpProvider
    {
        public HttpProvider( string rootDomain )
        {
            RootDomain = rootDomain;
        }

        public string RootDomain { get; private set; }

        public HttpWebRequest CreateRequest( string url , string method = "GET" )
        {
            HttpWebRequest request;
            request = (HttpWebRequest) WebRequest.Create( $"{RootDomain}?{url}" );

            request.Method = method;
            return request;
        }

        public string ExecuteRequest( string url )
        {
            var request = CreateRequest( url );

            var response = (HttpWebResponse) request.GetResponse( );
            var stream = response.GetResponseStream( );

            try
            {
                var data = new StreamReader( stream ).ReadToEnd( );
                return data;
            }
            finally
            {
                stream.Close( );
            }


        }
    }
}
