using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPCore.Base
{
    public abstract class BaseFeatureReceiver : SPFeatureReceiver
    {
        /// <summary>
        /// Gets the feature scope of the calling feature.
        /// </summary>
        /// <value>The scope.</value>
        public SPFeatureScope Scope { get; private set; }

        public SPFarm Farm { get; private set; }

        public SPWebApplication WebApplication { get; private set; }

        /// <summary>
        /// Gets the <see cref = "SPSite" /> that contains the feature.
        /// </summary>
        public SPSite Site { get; private set; }

        /// <summary>
        /// Gets the <see cref = "SPWeb" /> that contains the feature.
        /// </summary>
        public SPWeb Web { get; private set; }

        /// <summary>
        /// Gets the feature receiver properties.
        /// </summary>
        /// <value>
        /// The feature receiver properties.
        /// </value>
        protected SPFeatureReceiverProperties Properties { get; private set; }

        private void Init(SPFeatureReceiverProperties properties)
        {
            this.Properties = properties;
            this.Scope = properties.Definition.Scope;

            switch (this.Scope)
            {
                case SPFeatureScope.Farm:
                    this.Farm = properties.Feature.Parent as SPFarm;
                    this.WebApplication = null;
                    this.Site = null;
                    this.Web = null;
                    break;
                case SPFeatureScope.WebApplication:
                    this.WebApplication = properties.Feature.Parent as SPWebApplication;
                    this.Farm = this.WebApplication.Farm;
                    this.Site = null;
                    this.Web = null;
                    break;
                case SPFeatureScope.Site:
                    this.Site = properties.Feature.Parent as SPSite;
                    this.Web = this.Site.RootWeb;
                    this.WebApplication = this.Site.WebApplication;
                    this.Farm = this.WebApplication.Farm;
                    break;
                case SPFeatureScope.Web:
                    this.Web = properties.Feature.Parent as SPWeb;
                    this.Site = this.Web.Site;
                    this.WebApplication = this.Site.WebApplication;
                    this.Farm = this.WebApplication.Farm;
                    break;
            }
        }


        /// <summary>
        /// Occurs on feature activation.
        /// </summary>
        /// <param name="properties">
        /// An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"/> object that represents the properties of the event.
        /// </param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);
            Init(properties);
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            base.FeatureDeactivating(properties);
            Init(properties);
        }

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            base.FeatureInstalled(properties);
            Init(properties);
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            base.FeatureUninstalling(properties);
            Init(properties);
        }

        public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        {
            base.FeatureUpgrading(properties, upgradeActionName, parameters);
            Init(properties);
        }
    }
}
