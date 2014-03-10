using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SPCore.Helper;
using SPCore.Logging;

namespace SPCore.Base
{
    public abstract class BaseJobDefinition : SPJobDefinition
    {
        readonly Logger _logger;

        #region [Properties]

        public ActionScope ActionScope
        {
            get
            {
                if (Properties["ActionScope"] != null)
                {
                    return (ActionScope)Properties["ActionScope"];
                }

                return ActionScope.AllWebs;
            }
            set { Properties["ActionScope"] = value; }
        }

        public string WebUrl
        {
            get
            {
                if (Properties["WebUrl"] != null)
                {
                    return (string)Properties["WebUrl"];
                }

                return string.Empty;
            }
            set { Properties["WebUrl"] = value; }
        }

        public abstract string DefaultJobName { get; }
        public abstract string DefaultJobTitle { get; }

        #endregion

        #region [Constructors]

        protected BaseJobDefinition()
            : base()
        {
            if (!string.IsNullOrEmpty(DefaultJobTitle))
            {
                Title = DefaultJobTitle;
            }

            _logger = new Logger(Title);
        }

        protected BaseJobDefinition(string jobName, SPService service, SPServer server, SPJobLockType targetType) :
            base(jobName, service, server, targetType)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                Name = string.Format("{0}", DefaultJobName);
            }

            if (!string.IsNullOrEmpty(DefaultJobTitle))
            {
                Title = DefaultJobTitle;
            }

            _logger = new Logger(Title);
        }

        protected BaseJobDefinition(string jobName, SPWebApplication webApp)
            : base(jobName, webApp, null, SPJobLockType.Job)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                Name = string.Format("{0}_{1}", DefaultJobName, webApp.Id);
            }

            if (!string.IsNullOrEmpty(DefaultJobTitle))
            {
                Title = DefaultJobTitle;
            }

            _logger = new Logger(Title);
        }

        #endregion

        #region [Public methods]

        public override void Execute(Guid targetInstanceId)
        {
            if (WebApplication == null) return;

            if (!string.IsNullOrEmpty(WebUrl))
            {
                SPHelper.RunAction(ExecuteOnWeb, this.WebUrl, ActionScope, null);
            }
            else
            {
                WebApplication.Sites.AsSafeEnumerable().ForEach(
                   site => SPHelper.RunAction(ExecuteOnWeb, site, ActionScope));
            }
        }

        protected abstract void Execute(SPWeb web);

        private void ExecuteOnWeb(SPWeb web)
        {
            try
            {
                Execute(web);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public static void Uninstall<T>(SPWebApplication webApp)
            where T : BaseJobDefinition
        {
            foreach (T job in webApp.JobDefinitions.OfType<T>())
            {
                job.Delete();
            }
        }

        public static void Uninstall(SPWebApplication webApp, string jobName)
        {
            foreach (SPJobDefinition job in webApp.JobDefinitions.Where(job => job.Name == jobName))
            {
                job.Delete();
            }
        }

        #endregion

    }
}
