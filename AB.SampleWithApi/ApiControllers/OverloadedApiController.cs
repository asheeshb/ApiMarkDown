using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AB.SampleWithApi.ApiControllers
{
    public class OverloadedApiController : ApiController
    {
        /// <summary>
        /// Save Some Data somewhere
        /// </summary>
        /// <param name="a">input of json format </param>
        /// <returns>
        /// [{ "name": "propertyValue", "test": "propertyValue"  }]
        /// </returns>
        /// <example>
        ///  [{ "name": "propertyValue", "test": "propertyValue"  }]
        /// </example>
        [HttpPost]
        public HttpResponse SaveSomeDataWithOverload([FromBody]string a)
        {

            HttpResponse r = null;


            return r;
        }


        /// <summary>
        ///  Saves some data to server.
        /// </summary>
        /// <example></example>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponse SaveSomeDataWithOverload([FromBody]string a, string b)
        {
            HttpResponse r = null;


            return r;
        }
        
    }
}
