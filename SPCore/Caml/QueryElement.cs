﻿using System.Xml.Linq;

namespace SPCore.Caml
{
    public abstract class QueryElement
    {
        public string ElementName { get; private set; }

        protected QueryElement(string elementName)
        {
            ElementName = elementName;
        }

        protected QueryElement(string elementName, string existingElement)
        {
            ElementName = elementName;
            Parse(existingElement);
        }

        protected QueryElement(string elementName, XElement existingElement)
        {
            ElementName = elementName;
            Parse(existingElement);
        }

        public virtual XElement ToXElement()
        {
            return new XElement(ElementName);
        }

        protected abstract void OnParsing(XElement existingElement);

        private void Parse(XElement existingElement)
        {
            if (existingElement.Name.LocalName == ElementName && (existingElement.HasAttributes || existingElement.HasElements))
            {
                OnParsing(existingElement);
            }
        }

        private void Parse(string existingElement)
        {
            XElement el = XElement.Parse(existingElement, LoadOptions.PreserveWhitespace);
            Parse(el);
        }
    }
}
