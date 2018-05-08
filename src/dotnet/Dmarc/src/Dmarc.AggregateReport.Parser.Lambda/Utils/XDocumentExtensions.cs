using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Dmarc.AggregateReport.Parser.Lambda.Utils
{
    public static class XDocumentExtensions
    {
        public static XElement Single(this XDocument document, string name)
        {
            return document.Elements().Single(_ => _.Name.LocalName == name);
        }

        public static XElement FirstOrDefault(this XDocument document, string name)
        {
            return document.Elements().FirstOrDefault(_ => _.Name.LocalName == name);
        }

        public static XElement FirstOrDefault(this XElement element, string name)
        {
            return element.Elements().FirstOrDefault(_ => _.Name.LocalName == name);
        }

        public static XElement SingleOrDefault(this XElement element, string name)
        {
            return element.Elements().SingleOrDefault(_ => _.Name.LocalName == name);
        }

        public static XElement Single(this XElement element, string name)
        {
            return element.Elements().Single(_ => _.Name.LocalName == name);
        }

        public static IEnumerable<XElement> Where(this XElement element, string name)
        {
            return element.Elements().Where(_ => _.Name.LocalName == name);
        }

        public static IEnumerable<XElement> Where(this XDocument document, string name)
        {
            return document.Elements().Where(_ => _.Name.LocalName == name);
        }
    }
}