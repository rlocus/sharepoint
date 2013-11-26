using Microsoft.Office.Server.Search.Administration;
using System.Collections;

namespace SPCore.Search
{
    public class SPManagedPropertyCollection : IEnumerable
    {
        private readonly ManagedPropertyCollection _propertyCollection;

        public SPManagedPropertyCollection(ManagedPropertyCollection propertyCollection)
        {
            _propertyCollection = propertyCollection;
        }

        public int Count { get { return _propertyCollection.Count; } }

        public object this[string managedPropertyName] { get { return _propertyCollection[managedPropertyName]; } }

        public bool Contains(string name)
        {
            return _propertyCollection.Contains(name);
        }

        public ManagedProperty Create(string name, ManagedDataType managedType)
        {
            return _propertyCollection.Create(name, managedType);
        }
        public ManagedProperty CreateCrawlMonProperty()
        {
            return _propertyCollection.CreateCrawlMonProperty();
        }

        public IEnumerator GetEnumerator()
        {
            return _propertyCollection.GetEnumerator();
        }

    }
}
