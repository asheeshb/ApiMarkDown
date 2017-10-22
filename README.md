# ApiMarkDown:

This project aims towards creating MarkDown documentation for WebAPI functions written in .NET.

It uses the comments added to the function.

Prerequsite:

Any .NET library or project.

In the Properties of the project, under Build tab set check on the XMLDocumentation file in the output section.
(this will build a documenataion XML for the project which can be used mine the comments for the API controller methods).

# Project Structure:

**Bajra.ApiMdGenerator** : Core library project which uses the .dll and .xml to generate MD files

**Bajra.ApiMdRunner**    : Executable projects which makes use of the ApiMdGenerator library.

**AB.SampleWithApi**     : Sample project for which the MD files will be generated.


