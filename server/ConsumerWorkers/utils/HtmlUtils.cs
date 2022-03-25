using HtmlAgilityPack;

namespace Chronoria_ConsumerWorkers.utils
{
    public class HtmlUtils
    {
        public static string? GetTitle(string htmlContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var subjectAttr = htmlDoc.DocumentNode.SelectSingleNode("//title");
            if (subjectAttr == null)
            {
                return null;
            }
            string title = subjectAttr.InnerText;
            return title;
        }
    }
}
