using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using SPCore.Helper;

namespace SPCore.Base
{
    public abstract class BaseJobDefinition : SPJobDefinition
    {
        #region [Properties]

        public IEnumerable<string> WebUrls
        {
            get
            {
                return GetWebUrls((string)Properties["WebUrls"]);
            }
            set
            {
                Properties["WebUrls"] = GetWebUrlString(WebApplication, value);
            }
        }

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

        public abstract string DefaultJobName { get; }
        public abstract string DefaultJobTitle { get; }

        ////public event EventHandler OnExecuted;

        #endregion

        #region [Constructors]

        protected BaseJobDefinition()
            : base()
        {
            if (!string.IsNullOrEmpty(DefaultJobTitle))
            {
                Title = DefaultJobTitle;
            }
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
        }

        protected BaseJobDefinition(string jobName, IEnumerable<string> webUrls, SPWebApplication webApp)
            : base(jobName, webApp, null, SPJobLockType.Job)
        {
            WebUrls = webUrls;

            if (string.IsNullOrEmpty(jobName))
            {
                Name = string.Format("{0}_{1}", DefaultJobName, webApp.Id);
            }

            if (!string.IsNullOrEmpty(DefaultJobTitle))
            {
                Title = DefaultJobTitle;
            }
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
        }

        #endregion

        #region [Public methods]

        public override void Execute(Guid targetInstanceId)
        {
            if (WebApplication == null) return;

            IEnumerable<string> urls = WebUrls;

            if (urls.Count() == 0)
            {
                //ActionScope = ActionScope.AllWebs;
                WebApplication.Sites.AsSafeEnumerable().ForEach(
                    site => SPHelper.RunAction(ExecuteOnWeb, site, ActionScope));
            }
            else
            {
                //if (ActionScope == ActionScope.AllWebs || ActionScope == ActionScope.RootWeb)
                //{
                //    ActionScope = ActionScope.Web;
                //}

                foreach (string url in urls)
                {
                    SPHelper.RunAction(ExecuteOnWeb, url, ActionScope, null);
                }
            }

            ////Executed();
        }

        //protected virtual void Executed()
        //{
        //    if (OnExecuted != null)
        //    {
        //        OnExecuted(this, EventArgs.Empty);
        //    }
        //}

        protected abstract void Execute(SPWeb web);

        private void ExecuteOnWeb(SPWeb web)
        {
            try
            {
                Execute(web);
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                                                      new SPDiagnosticsCategory(this.Name, TraceSeverity.Unexpected,
                                                                                EventSeverity.Error),
                                                      TraceSeverity.Unexpected,
                                                      string.Format("Web Url = {0}: {1}", web.Url, ex.Message),
                                                      ex.StackTrace);
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

        #region [Helper methods]

        private static IEnumerable<string> GetWebUrls(string urls)
        {
            return !string.IsNullOrEmpty(urls)
                       ? urls.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(url => url.Trim()).ToArray()
                       : new string[0];
        }

        private static string GetWebUrlString(SPWebApplication webApplication, IEnumerable<string> urls)
        {
            string webUrls = string.Empty;

            if (urls != null)
            {
                foreach (string url in urls)
                {
                    if (string.IsNullOrEmpty(url)) { continue; }

                    string uriHost = webApplication.GetResponseUri(SPUrlZone.Default).AbsoluteUri;

                    if (SPUrlUtility.IsUrlRelative(url))
                    {
                        string webUrl = SPUrlUtility.CombineUrl(uriHost, url);
                        webUrls += webUrl + ",";
                    }
                    else
                    {
                        if (url.StartsWith(uriHost))
                        {
                            webUrls += url + ",";
                        }
                    }
                }

                webUrls = webUrls.TrimEnd(',');
            }

            return webUrls;
        }
        #endregion
    }
}
