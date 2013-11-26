using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;

namespace SPCore
{
    public static class SPWebCollectionExtensions
    {
        public static void ForEach(this SPWebCollection source, Action<SPWeb> action)
        {
            source.AsSafeEnumerable().ForEach(action);
        }

        public static IEnumerable<SPWeb> GetWebs(this SPWebCollection source, Func<SPWeb, bool> predicate)
        {
            return source.AsSafeEnumerable().Where(predicate);
        }

        #region AsSafeEnumerable

        /// <summary>
        /// Returns a collection of <see cref="SPWeb"/> objects that will be disposed by its <see cref="IEnumerator"/>.
        /// </summary>
        /// <param name="webs">The <see cref="SPWebCollection"/> that contains the sites to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{SPWeb}"/> that will dispose each <see cref="SPWeb"/> returned.</returns>
        public static IEnumerable<SPWeb> AsSafeEnumerable(this SPWebCollection webs)
        {
            return webs.AsSafeEnumerable(null);
        }

        public static IEnumerable<SPWeb> AsSafeEnumerable(this SPWebCollection webs, Action<SPWeb> onDispose)
        {
            foreach (SPWeb web in webs)
                try
                {
                    yield return web;
                }
                finally
                {
                    if (web != null)
                    {
                        if (onDispose != null)
                            onDispose(web);
                        web.Dispose();
                    }
                }
        }

        #endregion

    }
}
