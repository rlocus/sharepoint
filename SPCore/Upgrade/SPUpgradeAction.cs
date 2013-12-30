using System;
using Microsoft.SharePoint;

namespace SPCore.Upgrade
{
    public abstract class SPUpgradeAction : ISPUpgradeAction
    {
        public virtual Version BeginVersion
        {
            get;
            private set;
        }

        public virtual Version EndVersion
        {
            get;
            private set;
        }

        protected SPUpgradeAction()
        {
            BeginVersion = new Version(0, 0, 0, 0);
            EndVersion = new Version(0, 0, 0, 0);
        }

        private void Upgrade(Version version, SPSite site, SPWeb web)
        {
            if (InRange(version))
            {
                OnUpgrading(site, web);
            }
        }

        protected abstract void OnUpgrading(SPSite site, SPWeb web);

        protected bool InRange(Version version)
        {
            return !(null != version) || ((null == BeginVersion || BeginVersion <= version) && (null == EndVersion || EndVersion > version));
        }

        public void Upgrade(Version version)
        {
            SPSite site = null;
            SPWeb web = null;

            if (SPContext.Current != null)
            {
                site = SPContext.Current.Site;
                web = SPContext.Current.Web;
            }

            Upgrade(version, site, web);
        }

        public void Upgrade(Version version, SPSite site)
        {
            if (site != null)
            {
                SPWeb web = site.RootWeb;
                Upgrade(version, site, web);
            }
            else
            {
                Upgrade(version);
            }
        }

        public void Upgrade(Version version, SPWeb web)
        {
            if (web != null)
            {
                SPSite site = web.Site;
                Upgrade(version, site, web);
            }
            else
            {
                Upgrade(version);
            }
        }
    }
}
