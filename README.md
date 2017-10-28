# ApiMarkDown:

Using this project one can create quick Mark Down Web API documentations for .NET APIs methods.

Features:-

* Generate standard MD files by running through the .NET assembly source.
* Uses the XML based comments within the project to detail out the documentation.
* Each generation creates a new version of MD files organized by folders based on the ApiControllers.
* Supports for overridden methods.
* Support for reuse of manually added text in the generated MD file to get inserted within the next MD generation cycle. *However this is supported for the *Note* section only*.


**Prerequisite**:

Any .NET library or project which have classes inheriting [System.Web.Http.ApiController](https://msdn.microsoft.com/en-us/library/system.web.http.apicontroller(v=vs.118).asp)

Most of the documentation in MD is generated using the comments added to the api methods in the .NET class.
However this is optional.

If comments are there in Api Method and you want to use that data, you need to enable the generation of the XML document file when the Dll is generated.

Use the below mention steps:-

* Go to the **Properties** of the project
* Select **Build tab** 
* Set check on the **XMLDocumentation file** in the **output section**.

# Project Structure:

**Bajra.ApiMdGenerator** : Core library project which uses the .dll and .xml to generate MD files

**Bajra.ApiMdRunner**    : Executable projects which makes use of the ApiMdGenerator library.

**AB.SampleWithApi**     : Sample project for which the MD files will be generated.

# Output Structure:

 By Default the MD files are output to folder **C:\MDFiles\MarkDown_[CurrentDateTimeHere]\\**

this can be configured in App.config

# Note:

The project reference for **System.Web.Http** must match the version that is reference within the .NET assembly for which the MD files are to be generated.

If required remove the existing **System.Web.Http** reference and point to any specific version of the same DLL.

# References:

https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/recommended-tags-for-documentation-comments

# Example :

For example the following style of commenting can generate the 95% of the MD documentation for an API.

```csharp

        /// <summary>
        /// Save Some Data somewhere <para> test </para>
        /// Test next line
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
```