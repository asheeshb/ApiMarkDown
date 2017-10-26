using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AB.SampleWithApi.ApiControllers
{
    public class BasicTestApiController : ApiController
    {
        /// <summary>
        /// Save Some Data somewhere <para> test </para>
        /// Test
        /// <see cref="AB.SampleWithApi.MyDto"/>
        /// </summary>
        /// <param name="a">input of json format </param>
        /// <returns>
        ///     some other return text 
        ///     <para>For fun</para> text here also
        ///     <para>SUCCESS</para>
        ///         <para>When Success</para>
        ///         <code>
        ///             [{ "name": "propertyValue", "test": "propertyValue"  }]
        ///         </code>
        ///     <para>SUCCESS</para>
        ///         <para>When Success Again</para>
        ///         <code>
        ///             [{ "name": "propertyValue", "test": "propertyValue"  }]
        ///         </code>
        ///     <para>FAIL</para>
        ///         <para>Code: 401 UNAUTHORIZED</para>
        ///         <code>
        ///             {}
        ///         </code>
        ///         <para>Code: 401 UNAUTHORIZED</para>
        ///         <code>
        ///             {}
        ///         </code>
        /// </returns>
        /// <example>
        ///  call example
        /// </example>
        [HttpPost]
        public HttpResponse SaveSomeData([FromBody]string a)
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
        public HttpResponse SaveAnotherDataToServer([FromBody]string a, string b)
        {
            HttpResponse r = null;


            return r;
        }


        /// <summary>
        /// gets the Count of all record
        /// </summary>
        /// <returns></returns>
        public HttpResponse CountAllRecord()
        {
            HttpResponse r = null;

            return r;
        }

    }
}
