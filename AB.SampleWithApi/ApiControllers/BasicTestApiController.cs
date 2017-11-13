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
        /// Test next line
        /// <see cref="AB.SampleWithApi.MyDto"/>
        /// </summary>
        /// <param name="jsonData">
        ///     input of json format 
        ///     <code>
        ///         [{name:'aaa'}]
        ///     </code>
        /// </param>
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
        public HttpResponse SaveSomeData([FromBody]string jsonData)
        {

            HttpResponse r = null;


            return r;
        }


        /// <summary>
        ///  Saves some data to server.
        /// </summary>
        /// <example></example>
        /// <param name="userName"></param>
        /// <param name="someJsonString">
        ///     input of json format 
        ///     <code>
        ///         [{name:'aaa'}]
        ///     </code>
        /// </param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponse SaveAnotherDataToServer([FromBody]string userName, string someJsonString)
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

        /// <summary>
        /// Test nullable integer
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public HttpResponse CountTotal(int? count = null)
        {
            HttpResponse r = null;

            return r;
        }

    }
}
