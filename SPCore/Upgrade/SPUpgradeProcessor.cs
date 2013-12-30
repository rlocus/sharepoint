using System;
using Microsoft.SharePoint;
using SPCore.Helper;

namespace SPCore.Upgrade
{
    public sealed class SPUpgradeProcessor
    {
        private readonly Version _version;
        private readonly ISPUpgradeAction[] _actions;
        private readonly SPWeb _web;

        public ActionScope Scope { get; set; }

        public SPUpgradeProcessor(Version version, params ISPUpgradeAction[] actions)
        {
            _version = version;
            _actions = actions;

            if (SPContext.Current != null)
            {
                _web = SPContext.Current.Web;
            }
        }

        public SPUpgradeProcessor(Version version, SPWeb web, params ISPUpgradeAction[] actions)
            : this(version, actions)
        {
            _web = web;
        }

        public void Run()
        {
            if (_actions == null) return;

            foreach (ISPUpgradeAction spUpgradeAction in _actions)
            {
                ISPUpgradeAction action = spUpgradeAction;
                SPHelper.RunAction((web) => action.Upgrade(_version, web), _web, Scope);
            }
        }
    }
}
