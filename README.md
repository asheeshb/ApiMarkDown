# ApiMarkDown:

This project aims towards creating MarkDown documentation for WebAPI functions written in .NET.

It uses the comments added to the function.

Prerequsite:

Any .NET library or project.

In the **Properties** of the project, under **Build tab** set check on the **XMLDocumentation file** in the **output section**.
(this will build a documenataion XML for the project which can be used mine the comments for the API controller methods).

# Project Structure:

**Bajra.ApiMdGenerator** : Core library project which uses the .dll and .xml to generate MD files

**Bajra.ApiMdRunner**    : Executable projects which makes use of the ApiMdGenerator library.

**AB.SampleWithApi**     : Sample project for which the MD files will be generated.

# Output Structure:

 By Default the MD files are output to folder **C:\MDFiles\MarkDown_[CurrentDateTimeHere]\\**

# Note:

The project reference for **System.Web.Http** must match the version that is reference within the .NET assemply for which the MD files are to be generated.

If required remove the existing **System.Web.Http** reference and point to any specific version of the same DLL.

# References:

https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/recommended-tags-for-documentation-comments
