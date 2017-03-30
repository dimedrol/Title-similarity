using Data;
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
            provider.GetArticles( Location.StPetersburg , 10000 );
        }
    }
}
