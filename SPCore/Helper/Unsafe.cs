using System;
using Microsoft.SharePoint;

namespace SPCore.Helper
{
    /// <summary>
    /// Class to be able to automatically allow unsafe update and reset the value with
    /// using(new Unsafe(web)){}
    /// </summary>
    public sealed class Unsafe : IDisposable
    {
        private readonly bool _originalAllowUnsafeUpdates;
        private readonly SPWeb _web;
        private readonly SPSite _site;

        /// <summary>
        /// Constructor where we store the original value of AllowUnsafeUpdates
        /// </summary>
        /// <param name="originalWeb">The WebSite to make unsafe updates</param>
        public Unsafe(SPWeb originalWeb)
        {
            if (originalWeb == null) throw new ArgumentNullException("originalWeb");
            this._web = originalWeb;
            this._originalAllowUnsafeUpdates = originalWeb.AllowUnsafeUpdates;
            this._web.AllowUnsafeUpdates = true;
        }

        public Unsafe(SPSite originalSite)
        {
            if (originalSite == null) throw new ArgumentNullException("originalSite");
            this._site = originalSite;
            this._originalAllowUnsafeUpdates = originalSite.AllowUnsafeUpdates;
            this._site.AllowUnsafeUpdates = true;
        }

        /// <summary>
        /// Destructor where we reset to original value of the AllowUnsafeUpdates
        /// </summary>
        public void Dispose()
        {
            if (_site != null)
            {
                this._site.AllowUnsafeUpdates = this._originalAllowUnsafeUpdates;
            }

            if (_web != null)
            {
                this._web.AllowUnsafeUpdates = this._originalAllowUnsafeUpdates;
            }
        }
    }
}
