using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPCore
{
    public static class SPSiteCollectionExtensions
    {
        public static void ForEach(this SPSiteCollection source, Action<SPSite> action)
        {
            source.AsSafeEnumerable().ForEach(action);
        }

        #region AsSafeEnumerable

        /// <summary>
        /// Returns a collection of <see cref="SPSite"/> objects that will be disposed by its <see cref="IEnumerator"/>.
        /// </summary>
        /// <param name="sites">The <see cref="SPSiteCollection"/> that contains the sites to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{SPSite}"/> that will dispose each <see cref="SPSite"/> returned.</returns>
        public static IEnumerable<SPSite> AsSafeEnumerable(this SPSiteCollection sites)
        {
            return sites.AsSafeEnumerable(null);
        }

        public static IEnumerable<SPSite> AsSafeEnumerable(this SPSiteCollection sites, Action<SPSite> onDispose)
        {
            foreach (SPSite site in sites)
                try
                {
                    yield return site;
                }
                finally
                {
                    if (site != null)
                    {
                        if (onDispose != null)
                            onDispose(site);
                        site.Dispose();
                    }
                }
        }

        #endregion

    }
}
