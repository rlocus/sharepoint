using System;
using Microsoft.Office.Server.Search.Query;
using Microsoft.Office.Server.Search.WebControls;
using Microsoft.SharePoint;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search
{
    /// <summary>
    /// A generic class to encapuslate different search query methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericSearchQuery<T> : IDisposable where T : Query
    {
        #region Fields
        private readonly T _query;
        private readonly CoreResultsWebPart _coreResultsWebPart;
        #endregion

        #region Properties

        public string Scope { get; set; }

        public T Query
        {
            get { return _query; }
        }
        #endregion

        #region ctor
        public GenericSearchQuery(SPSite site)
        {
            _query = (T)Activator.CreateInstance(typeof(T), site);
        }
        public GenericSearchQuery(SPSite site, CoreResultsWebPart coreResultsWebPart)
            : this(site)
        {
            _coreResultsWebPart = coreResultsWebPart;
            SyncWithCore();
        }
        #endregion

        #region Public methods
        public ResultTableCollection Execute(ISearchQuery query)
        {
            _query.QueryText = query.ToString();
            return _query.Execute();
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _query.Dispose();
        }

        #endregion

        #region Private methods
        private void SyncWithCore()
        {
            if (_coreResultsWebPart != null)
            {
                _query.TrimDuplicates = _coreResultsWebPart.RemoveDuplicates;
                _query.EnableStemming = _coreResultsWebPart.EnableStemming;
                _query.IgnoreAllNoiseQuery = _coreResultsWebPart.IgnoreNoise;
                if (!string.IsNullOrEmpty(_coreResultsWebPart.Scope))
                    Scope = _coreResultsWebPart.Scope;
            }
        }
        #endregion
    }
}
