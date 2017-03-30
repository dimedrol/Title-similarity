using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers
{
    public class WikiProvider
    {
        protected string _RootDomain;

        public WikiProvider( ) : this( "en" )
        {

        }

        public WikiProvider( string language )
        {
            _RootDomain = $"https://{language}.wikipedia.org/w/api.php";
        }

        protected string ExecuteRequest( string url )
        {
            var httpProvider = new HttpProvider( _RootDomain );
            var data = httpProvider.ExecuteRequest( url );

            return data;
        }
    }
}
