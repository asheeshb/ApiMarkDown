using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator.MarkDownTemplates
{
    class BasicApiTemplate
    {
        public static StringBuilder GetTemplateStringBuilder(bool isPostApi, bool hasExample, string dateTimeStamp, string versionText, string imagePath = "")
        {

            string imgLogo = $@"

 ![Company Logo here]({imagePath})

";

            string templateMain = $@"
# {TemplateConsts.PLACEHOLDER_API_NAME}

* **Documentation Generated On** : {dateTimeStamp}
* **DLL Version** : {versionText}     

----

{TemplateConsts.PLACEHOLDER_ADDITIONAL_INFO}

----

### Controller : `{TemplateConsts.PLACEHOLDER_CONTROLLER_NAME}`

### URL : `{TemplateConsts.PLACEHOLDER_URL}`

### Method : {TemplateConsts.PLACEHOLDER_METHOD_TYPE}

----

### URL Params :

* ***Required***
 
{TemplateConsts.PLACEHOLDER_PARAM_LIST_REQUIRED}

* ***Optional:***
 
{TemplateConsts.PLACEHOLDER_PARAM_LIST_OPTIONAL}
";
            //----------------------------------------------
            string postStr = $@"
### Data Params :

{TemplateConsts.PLACEHOLDER_PARAM_FROM_BODY}

----

";
            //----------------------------------------------
            string strResponse = $@"
### Returns : 

{TemplateConsts.PLACEHOLDER_RETURN_GENERIC_RESPONSE}

* **Success Response:**
  
{TemplateConsts.PLACEHOLDER_SUCCESS_RESPONSE}
 
* **Error Response:**

{TemplateConsts.PLACEHOLDER_FAIL_RESPONSE}

----

";
            //----------------------------------------------
            string strExample = $@"
### Example :

{TemplateConsts.PLACEHOLDER_EXAMPLE}

----


";
            //----------------------------------------------
            string strNotes = $"{TemplateConsts.NOTE_SIGNATURE}<<<NOTES>>>\r\n";
            //----------------------------------------------

            StringBuilder sbr = new StringBuilder();

            if (!string.IsNullOrEmpty(imagePath))
                sbr.Append(imagePath);

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
