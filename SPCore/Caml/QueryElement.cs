using System.Xml.Linq;

namespace SPCore.Caml
{
    public abstract class QueryElement
    {
        public string ElementName { get; private set; }

        protected QueryElement(string elementName)
        {
            ElementName = elementName;
        }

        public virtual XElement ToXElement()
        {
            return new XElement(ElementName);
        }
    }
}
