using Markdig;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator
{
    public static class MdToHtml
    {
        public static void GenerateHtmlFile(string markdownFile)
        {
            string content = File.ReadAllText(markdownFile);
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var html = Markdown.ToHtml(content, pipeline).Replace("\n", Environment.NewLine);

            string htmlFileName = GetHtmlFileName(markdownFile);
            html = CreateFromHtmlTemplate(markdownFile, content, html);

            File.WriteAllText(htmlFileName, html, new UTF8Encoding(true));
        }

        private static string GetHtmlFileName(string markdownFile)
        {
            return Path.ChangeExtension(markdownFile, ".html");
        }
        
        private static string CreateFromHtmlTemplate(string markdownFile, string content, string html)
        {
            try
            {
                //string templateFileName = GetHtmlTemplate(markdownFile);
                string template = //File.ReadAllText(templateFileName);
                    @"
<!DOCTYPE html>
<html>
<head>
    <title>[title]</title>
</head>
<body>

    [content]

</body>
</html>
";

                var doc = Markdown.Parse(content);

                string title = Path.GetFileNameWithoutExtension(markdownFile);
                
                return template.Replace("[title]", title).Replace("[content]", html);
            }
            catch (Exception ex)
            {
                return html;
            }
        }        
    }
}
