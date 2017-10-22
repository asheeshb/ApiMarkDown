using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator.MarkDownTemplates
{
    class BasicApiTemplate
    {
        public static StringBuilder GetTemplateStringBuilder(bool isPostApi, bool hasExample)
        {
            StringBuilder sbr = new StringBuilder();
            string templateMain = @"
**<<<API_NAME>>>**
----

<<<ADDITIONAL_INFORMATION>>>

----

* **URL** : `<<<URL>>>`

* **Method:**  <<<METHOD_TYPE>>>
  
* **URL Params**

   **Required:**
 
<<<PARAM_LIST_REQUIRED>>>

   **Optional:**
 
<<<PARAM_LIST_OPTIONAL>>>
";
            //----------------------------------------------
            string postStr = @"
* **Data Params**

<<<PARAM_FROM_BODY>>>
";
            //----------------------------------------------
            string strResponse = @"
* **Success Response:**
  
<<<SUCCESS_RESPONSE>>>
 
* **Error Response:**

<<<ERROR_RESPONSE>>>
";
            //----------------------------------------------
            string strExample = @"
* **Sample Call:**

<<<EXAMPLE>>>
";
            //----------------------------------------------
            string strNotes = @"
* **Notes:**

<<<NOTES>>>
";
            //----------------------------------------------

            sbr.Append(templateMain);
            if (isPostApi)
                sbr.Append(postStr);

            sbr.Append(strResponse);

            if (isPostApi)
                sbr.Append(strExample);

            sbr.Append(strNotes);

            return sbr;

        }
    }
}
