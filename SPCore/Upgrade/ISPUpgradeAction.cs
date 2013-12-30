using System;
using Microsoft.SharePoint;

namespace SPCore.Upgrade
{
    public interface ISPUpgradeAction
    {
        Version BeginVersion { get; }
        Version EndVersion { get; }
        void Upgrade(Version version);
        void Upgrade(Version version, SPSite site);
        void Upgrade(Version version, SPWeb web);
    }
}
