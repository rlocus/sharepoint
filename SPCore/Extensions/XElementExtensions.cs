using System.Xml;
using System.Xml.Linq;

namespace SPCore.Extensions
{
    internal static class XElementExtensions
    {
        public static XmlNode GetXmlNode(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }

        public static XmlElement GetXmlElement(this XElement element)
        {
            return GetXmlNode(element).FirstChild as XmlElement;
        }

        public static XElement GetXElement(this XmlNode node)
        {
            XDocument xDoc = new XDocument();

            using (XmlWriter xmlWriter = xDoc.CreateWriter())
            {
                node.WriteTo(xmlWriter);
                return xDoc.Root;
            }
        }
    }
}
