using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Workflow;
using SPCore.Helper;
using WebPart = System.Web.UI.WebControls.WebParts.WebPart;

namespace SPCore
{
    public static class SPWebExtensions
    {
        public static void RunAsSystem(this SPWeb web, Action<SPWeb> action)
        {
            if (action != null) web.Site.RunAsSystem(web.ID, action);
        }

        public static void RunAsUser(this SPWeb web, SPUser user, Action<SPWeb> action)
        {
            if (action != null) web.Site.RunAsUser(web.ID, user, action);
        }

        public static T SelectAsSystem<T>(this SPWeb web, Func<SPWeb, T> selector)
        {
            if (selector != null) return web.Site.SelectAsSystem(web.ID, selector);
            else return default(T);
        }

        public static T SelectAsUser<T>(this SPWeb web, SPUser user, Func<SPWeb, T> selector)
        {
            if (selector != null) return web.Site.SelectAsUser(web.ID, user, selector);
            else return default(T);
        }

        public static SPList GetListByUrl(this SPWeb web, string listUrl)
        {
            if (string.IsNullOrEmpty(listUrl)) throw new ArgumentNullException("listUrl");

            Uri uri = new Uri(listUrl, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri /*SPUrlUtility.IsUrlRelative(listUrl)*/)
            {
                string relativeListUrl = string.Concat("/", listUrl.TrimStart('/'));

                relativeListUrl = relativeListUrl.StartsWith(web.ServerRelativeUrl)
                                      ? relativeListUrl
                                      : SPUrlUtility.CombineUrl(web.ServerRelativeUrl, relativeListUrl);

                if (relativeListUrl == web.ServerRelativeUrl)
                {
                    throw new SPException("List does not exist.");
                }

                return web.GetList(relativeListUrl);
            }

            if (listUrl.TrimEnd('/') == web.Url)
            {
                throw new SPException("List does not exist.");
            }

            return web.GetList(listUrl);
        }

        public static SPList TryGetListByUrl(this SPWeb web, string listUrl)
        {
            try
            {
                return web.GetListByUrl(listUrl);
            }
            catch (Exception /*FileNotFoundException*/)
            {
                return null;
            }
        }

        public static SPList GetListByName(this SPWeb web, string strList)
        {
            SPList list = null;

            if (!string.IsNullOrEmpty(strList))
            {
                if (SPHelper.IsGuid(strList))
                {
                    list = web.Lists[new Guid(strList)];
                }
                else
                {
                    list = web.TryGetListByUrl(strList) ?? web.Lists[strList];
                }
            }

            return list;
        }

        public static IEnumerable<SPList> GetListsByContentType(this SPWeb web, SPContentTypeId contentTypeId)
        {
            return GetListsByContentType<SPList>(web, contentTypeId);
        }

        public static IEnumerable<TList> GetListsByContentType<TList>(this SPWeb web, SPContentTypeId contentTypeId)
            where TList : SPList
        {
            SPContentType contentType = web.AvailableContentTypes[contentTypeId];
            return SPHelper.GetListsByContentType<TList>(web, contentType);
        }

        public static IEnumerable<SPList> GetListsByContentType(this SPWeb web, string contentTypeName)
        {
            return GetListsByContentType<SPList>(web, contentTypeName);
        }

        public static IEnumerable<TList> GetListsByContentType<TList>(this SPWeb web, string contentTypeName)
             where TList : SPList
        {
            SPContentType contentType = web.AvailableContentTypes[contentTypeName];
            return SPHelper.GetListsByContentType<TList>(web, contentType);
        }

        public static SPContentType CreateContentType(this SPWeb web,
            SPContentTypeId parentContentTypeId,
            string ctName,
            Action<SPContentType> action,
            bool updateChildren = false)
        {
            using (SPWeb rootWeb = web.Site.OpenWeb(web.Site.RootWeb.ID))
            {
                SPContentType contentType = rootWeb.ContentTypes[ctName];

                if (contentType == null)
                {
                    SPContentType parentContentType = web.AvailableContentTypes[parentContentTypeId];

                    if (parentContentType != null)
                    {
                        contentType = new SPContentType(parentContentType, rootWeb.ContentTypes, ctName);
                        contentType = rootWeb.ContentTypes.Add(contentType);
                    }
                    else
                    {
                        throw new SPException(string.Format("Content type with Id = \"{0}\" not found or not available.",
                                                            parentContentTypeId));
                    }
                }

                if (action != null)
                {
                    action(contentType);
                }

                contentType.Update(updateChildren);
                return contentType;
            }
        }

        public static SPContentType CreateContentType(this SPWeb web,
          SPContentTypeId parentContentTypeId,
          string ctName,
          string group,
          Action<SPContentType> action,
          bool updateChildren = false)
        {
            return web.CreateContentType(parentContentTypeId, ctName,
                                         contentType =>
                                         {
                                             if (!string.IsNullOrEmpty(group))
                                             {
                                                 contentType.Group = group;
                                             }

                                             if (action != null)
                                             {
                                                 action(contentType);
                                             }
                                         },
                                         updateChildren);
        }

        public static TList AddList<TList>(this SPWeb web,
            string listName,
            string description,
            SPListTemplateType template,
            Action<TList> action)
            where TList : SPList
        {
            var list = (TList)web.Lists.TryGetList(listName);

            if (list == null)
            {
                Guid listId = web.Lists.Add(listName, description, template);
                list = (TList)web.Lists.GetList(listId, false);
            }

            if (action != null)
            {
                try
                {
                    action(list);
                }
                finally
                {
                    list.Update();
                }
            }

            return list;
        }

        public static TList AddList<TList>(this SPWeb web,
           string listName,
           string description,
           SPListTemplate template,
           Action<TList> action)
             where TList : SPList
        {
            var list = (TList)web.Lists.TryGetList(listName);

            if (list == null)
            {
                Guid listId = web.Lists.Add(listName, description, template);
                list = (TList)web.Lists.GetList(listId, false);
            }

            if (action != null)
            {
                try
                {
                    action(list);
                }
                finally
                {
                    list.Update();
                }
            }

            return list;
        }

        public static TList AddList<TList>(this SPWeb web,
           string internalListName,
           string displayListName,
           string description,
           SPListTemplateType template,
           Action<TList> action,
           bool onQuickLaunch = false)
             where TList : SPList
        {
            TList list;
            internalListName = SPHelper.RemoveIllegalUrlCharacters(internalListName);

            SPList existingList = web.Lists.TryGetList(displayListName);

            if (existingList != null)
            {
                if (existingList.RootFolder.Name != internalListName)
                {
                    throw new SPException(string.Format("List with Title=\"{0}\" already exists in the web [URL={1}].", displayListName, web.Url));
                }

                if (existingList.BaseTemplate != template)
                {
                    throw new SPException(string.Format("Existing list with Title=\"{0}\" was not created from a template \"{2}\" in the web [URL={1}].", displayListName, web.Url, (int)template));
                }

                list = (TList)existingList;
            }
            else
            {
                list = web.Lists.GetListsByInternalName<TList>(internalListName, template).SingleOrDefault();
            }

            if (list == null)
            {
                Guid listId = web.Lists.Add(internalListName, description, template);
                list = (TList)web.Lists.GetList(listId, false);
            }

            try
            {
                list.Title = displayListName;
                list.OnQuickLaunch = onQuickLaunch;

                if (action != null)
                {
                    action(list);
                }
            }
            finally
            {
                list.Update();
            }

            return list;
        }

        public static TList AddList<TList>(this SPWeb web,
           string internalListName,
           string displayListName,
           string description,
           SPListTemplate template,
           Action<TList> action,
           bool onQuickLaunch = false)
            where TList : SPList
        {
            TList list;
            internalListName = SPHelper.RemoveIllegalUrlCharacters(internalListName);

            SPList existingList = web.Lists.TryGetList(displayListName);

            if (existingList != null)
            {
                if (existingList.RootFolder.Name != internalListName)
                {
                    throw new SPException(string.Format("List with Title=\"{0}\" already exists in the web [URL={1}].", displayListName, web.Url));
                }

                if (existingList.BaseTemplate != template.Type)
                {
                    throw new SPException(string.Format("Existing list with Title=\"{0}\" was not created from a template \"{2}\" in the web [URL={1}].", displayListName, web.Url, (int)template.Type));
                }

                list = (TList)existingList;
            }
            else
            {
                list = web.Lists.GetListsByInternalName<TList>(internalListName, template).SingleOrDefault();
            }

            if (list == null)
            {
                Guid listId = web.Lists.Add(internalListName, description, template);
                list = (TList)web.Lists.GetList(listId, false);
            }

            try
            {
                list.Title = displayListName;
                list.OnQuickLaunch = onQuickLaunch;

                if (action != null)
                {
                    action(list);
                }
            }
            finally
            {
                list.Update();
            }

            return list;
        }

        public static string AddWebPartToPage(this SPWeb web, string webPartName, string pageUrl, string zoneId, int zoneIndex, Action<WebPart> action, out string errorMsg)
        {
            using (SPLimitedWebPartManager webPartManager = web.GetLimitedWebPartManager(pageUrl, PersonalizationScope.Shared))
            {
                using (WebPart webPart = CreateWebPart(web, webPartName, webPartManager, out errorMsg))
                {
                    webPartManager.AddWebPart(webPart, zoneId, zoneIndex);

                    if (action != null)
                    {
                        action(webPart);
                        webPartManager.SaveChanges(webPart);
                    }

                    return webPart.ID;
                }
            }
        }

        public static WebPart CreateWebPart(this SPWeb web, string webPartName, SPLimitedWebPartManager webPartManager, out string errorMsg)
        {
            var query = new SPQuery
                              {
                                  Query = String.Format(CultureInfo.CurrentCulture,
                                                        "<Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='File'>{0}</Value></Eq></Where>",
                                                        webPartName),
                                  RowLimit = 1
                              };

            SPList webPartGallery = web.IsRootWeb
                                        ? web.GetCatalog(SPListTemplateType.WebPartCatalog)
                                        : web.Site.RootWeb.GetCatalog(SPListTemplateType.WebPartCatalog);

            SPListItemCollection webParts = webPartGallery.GetItems(query);

            if (webParts.Count == 0)
            {
                throw new SPException(string.Format("Web Part \"{0}\" not found in the gallery.", webPartName));
            }

            using (Stream stream = webParts[0].File.OpenBinaryStream())
            {
                XmlReader xmlReader = new XmlTextReader(stream);
                WebPart webPart = webPartManager.ImportWebPart(xmlReader, out errorMsg);
                xmlReader.Close();
                return webPart;
            }
        }

        /// <summary>
        /// Finds the list template corresponding to the specified name
        /// </summary>
        /// <param name="web">The current web</param>
        /// <param name="templateName">The list template name</param>
        /// <returns>The list template</returns>
        public static SPListTemplate GetListTemplate(this SPWeb web, string templateName)
        {
            SPListTemplate template =
                web.ListTemplates.Cast<SPListTemplate>().FirstOrDefault(
                    lt => lt.Name.Equals(templateName, StringComparison.InvariantCultureIgnoreCase));
            return template;
        }

        public static IEnumerable<TList> GetLists<TList>(this SPWeb web, SPBaseType baseType, string templateName)
          where TList : SPList
        {
            SPListTemplate template = GetListTemplate(web, templateName);
            return web.GetLists<TList>(template.BaseType, template);
        }

        public static IEnumerable<TList> GetLists<TList>(this SPWeb web, SPBaseType baseType, SPListTemplate template)
           where TList : SPList
        {
            if (template == null) throw new ArgumentNullException("template");
            return web.GetLists<TList>(template.BaseType, template.Type);
        }

        public static IEnumerable<TList> GetLists<TList>(this SPWeb web, SPBaseType baseType, SPListTemplateType templateType)
           where TList : SPList
        {
            return web.GetLists<TList>(baseType).Where(lst => lst.BaseTemplate == templateType);
        }

        public static IEnumerable<TList> GetLists<TList>(this SPWeb web, SPBaseType baseType)
            where TList : SPList
        {
            return web.GetListsOfType(baseType).OfType<TList>();
        }

        public static SPWorkflowAssociation CreateListWorkflowAssociation(this SPWeb web, string listUrl, Guid workflowId, string assocName,
        string workflowTasksListName, string workflowHistoryListName,
        bool allowManual, bool autoStartCreate, bool autoStartChange, bool overwrite = false)
        {
            // Get the workflow template.
            SPWorkflowTemplate workflowTemplate =
                web.WorkflowTemplates.GetTemplateByBaseID(workflowId);

            return web.CreateListWorkflowAssociation(listUrl, workflowTemplate, assocName, workflowTasksListName,
                                                 workflowHistoryListName, allowManual, autoStartCreate, autoStartChange, overwrite);
        }

        public static SPWorkflowAssociation CreateListWorkflowAssociation(this SPWeb web, string listUrl, string workflowName, string assocName,
           string workflowTasksListName, string workflowHistoryListName,
           bool allowManual, bool autoStartCreate, bool autoStartChange, bool overwrite = false)
        {
            // Get the workflow template.
            SPWorkflowTemplate workflowTemplate = web.WorkflowTemplates.GetTemplateByName(workflowName, web.Locale);

            return web.CreateListWorkflowAssociation(listUrl, workflowTemplate, assocName, workflowTasksListName,
                                                 workflowHistoryListName, allowManual, autoStartCreate, autoStartChange, overwrite);
        }

        public static SPWorkflowAssociation CreateListWorkflowAssociation(this SPWeb web, string listUrl, SPWorkflowTemplate workflowTemplate, string assocName,
          string workflowTasksListName, string workflowHistoryListName,
          bool allowManual, bool autoStartCreate, bool autoStartChange, bool overwrite = false)
        {
            SPList workflowTasksList = web.Lists.TryGetList(workflowTasksListName) ??
                                       web.Lists[web.Lists.Add(workflowTasksListName,
                /*"This is workflow tasks list."*/ "", SPListTemplateType.Tasks)];

            SPList workflowHistoryList = web.Lists.TryGetList(workflowHistoryListName) ??
                                         web.Lists[web.Lists.Add(workflowHistoryListName,
                /*"This is workflow history list."*/"", SPListTemplateType.WorkflowHistory)];

            SPList list = web.GetListByUrl(listUrl);
            return list.CreateWorkflowAssociation(workflowTemplate, assocName, workflowTasksList,
                                                 workflowHistoryList, allowManual, autoStartCreate, autoStartChange, overwrite);
        }

        public static SPWorkflowAssociation CreateWorkflowAssociation(this SPWeb web, string listUrl, Guid workflowId, string assocName,
       string workflowTasksListName, string workflowHistoryListName,
       bool allowManual, bool autoStartCreate, bool autoStartChange)
        {
            // Get the workflow template.
            SPWorkflowTemplate workflowTemplate = web.WorkflowTemplates.GetTemplateByBaseID(workflowId);

            return web.CreateWorkflowAssociation(listUrl, workflowTemplate, assocName, workflowTasksListName,
                                                 workflowHistoryListName, allowManual, autoStartCreate, autoStartChange);
        }

        public static SPWorkflowAssociation CreateWorkflowAssociation(this SPWeb web, string listUrl, string workflowName, string assocName,
           string workflowTasksListName, string workflowHistoryListName,
           bool allowManual, bool autoStartCreate, bool autoStartChange)
        {
            // Get the workflow template.
            SPWorkflowTemplate workflowTemplate =
                web.WorkflowTemplates.GetTemplateByName(workflowName, CultureInfo.CurrentCulture);

            return web.CreateWorkflowAssociation(listUrl, workflowTemplate, assocName, workflowTasksListName,
                                                 workflowHistoryListName, allowManual, autoStartCreate, autoStartChange);
        }

        public static SPWorkflowAssociation CreateWorkflowAssociation(this SPWeb web, string listUrl, SPWorkflowTemplate workflowTemplate, string assocName,
          string workflowTasksListName, string workflowHistoryListName,
          bool allowManual, bool autoStartCreate, bool autoStartChange)
        {
            SPList workflowTasksList = web.Lists.TryGetList(workflowTasksListName) ??
                                       web.Lists[web.Lists.Add(workflowTasksListName,
                /*"This is workflow tasks list."*/ "", SPListTemplateType.Tasks)];

            SPList workflowHistoryList = web.Lists.TryGetList(workflowHistoryListName) ??
                                         web.Lists[web.Lists.Add(workflowHistoryListName,
                /*"This is workflow history list."*/"", SPListTemplateType.WorkflowHistory)];

            return web.CreateWorkflowAssociation(workflowTemplate, assocName, workflowTasksList,
                                                 workflowHistoryList, allowManual, autoStartCreate, autoStartChange);
        }

        public static SPWorkflowAssociation CreateWorkflowAssociation(this SPWeb web,
           SPWorkflowTemplate workflowTemplate, string assocName,
           SPList workflowTasksList, SPList workflowHistoryList,
           bool allowManual, bool autoStartCreate, bool autoStartChange, bool overwrite = false)
        {
            SPWorkflowAssociation workflowAssociation =
                web.CreateWorkflowAssociation(workflowTemplate, assocName, workflowTasksList, workflowHistoryList,
                                               overwrite,
                                               assoc =>
                                               {
                                                   assoc.AllowManual = allowManual;
                                                   assoc.AutoStartCreate = autoStartCreate;
                                                   assoc.AutoStartChange = autoStartChange;
                                               });
            return workflowAssociation;
        }

        public static SPWorkflowAssociation CreateWorkflowAssociation(this SPWeb web,
           SPWorkflowTemplate workflowTemplate, string assocName,
           SPList workflowTasksList, SPList workflowHistoryList,
           bool overwrite = false, Action<SPWorkflowAssociation> action = null)
        {
            if (workflowTemplate == null) throw new ArgumentNullException("workflowTemplate");

            SPWorkflowAssociation workflowAssociation = null;
            // create the association
            SPWorkflowAssociation assoc =
                SPWorkflowAssociation.CreateWebAssociation(
                    workflowTemplate, assocName,
                    workflowTasksList, workflowHistoryList);

            if (action != null)
            {
                action(assoc);
            }

            //apply the association to the list
            if (web != null)
            {
                if (overwrite)
                {
                    SPWorkflowAssociation dupWfAssoc =
                        web.WorkflowAssociations.GetAssociationByName(assocName, web.Locale);

                    if (dupWfAssoc != null)
                    {
                        if (dupWfAssoc.BaseId == workflowTemplate.Id)
                        {
                            web.WorkflowAssociations.Remove(dupWfAssoc);
                        }
                        else
                        {
                            throw new SPException(string.Format("Duplicate workflow name \"{0}\" is detected.", assocName));
                        }
                    }
                }

                workflowAssociation = web.WorkflowAssociations.Add(assoc);
            }

            return workflowAssociation;
        }

        public static IEnumerable<string> GetSubWebUrls(this SPWeb web, bool full = false)
        {
            return full
                       ? web.Webs.WebsInfo.Select(webInfo => web.Site.MakeFullUrl(webInfo.ServerRelativeUrl))
                       : web.Webs.WebsInfo.Select(webInfo => webInfo.ServerRelativeUrl);
        }

        public static SPWebTemplate GetWebTemplate(this SPWeb web, String templateName)
        {
            SPWebTemplateCollection wtc = web.GetAvailableWebTemplates(Convert.ToUInt32(web.Locale.LCID));
            return wtc.Cast<SPWebTemplate>().FirstOrDefault(wt => wt.Name.Equals(templateName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static DataTable GetData(this SPWeb web, SPQuery query, SPBaseType listType, bool recursive = false)
        {
            DataTable dt = web.GetSiteData(new SPSiteDataQuery()
                                 {
                                     Webs = recursive ? "<Webs Scope='Recursive' />" : "<Webs Scope='SiteCollection' />",
                                     Lists = string.Format("<Lists BaseType=\"{0}\" />", (int)listType),
                                     Query = query.Query,
                                     ViewFields = query.ViewFields,
                                     RowLimit = query.RowLimit,
                                     QueryThrottleMode = query.QueryThrottleMode
                                 });

            return dt;
        }

        public static DataTable GetData(this SPWeb web, SPQuery query, SPListTemplateType listType, bool recursive = false)
        {
            DataTable dt = web.GetSiteData(new SPSiteDataQuery()
            {
                Webs = recursive ? "<Webs Scope='Recursive' />" : "<Webs Scope='SiteCollection' />",
                Lists = string.Format("<Lists ServerTemplate=\"{0}\" />", (int)listType),
                Query = query.Query,
                ViewFields = query.ViewFields,
                RowLimit = query.RowLimit,
                QueryThrottleMode = query.QueryThrottleMode
            });

            return dt;
        }

        public static PublishingPage CreatePage(this SPWeb web, string pageName, string pageLayoutName)
        {
            if (!PublishingWeb.IsPublishingWeb(web)) { return null; }

            PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(web);

            PageLayout pageLayout =
                publishingWeb.GetAvailablePageLayouts().FirstOrDefault(
                    page => page.Name.Equals(pageLayoutName, StringComparison.InvariantCultureIgnoreCase));

            if (pageLayout == null)
            {
                return null;
                //throw new SPException(string.Format("Page layout with name {0} is not found.", pageLayoutName));
            }

            PublishingPage newPage = publishingWeb.AddPublishingPage(pageName, pageLayout);
            return newPage;
        }

        /// <summary>
        /// Checks if the custom action is contained in the SPUserCustomActionCollection and if so, 
        /// returns the id in the customActionId parameter
        /// </summary>
        /// <param name="web">The SharePoint web</param>
        /// <param name="customActionName">Name property given to the CustomAction element in the definition</param>
        /// <returns>The custom action Id or an empty string if not found</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Dependency-injected classes should expose non-static members only for consistency.")]
        public static Guid GetCustomActionIdForName(this SPWeb web, string customActionName)
        {
            Guid customActionId = default(Guid);

            foreach (SPUserCustomAction customAction in web.UserCustomActions.Where(customAction => string.Equals(customAction.Name, customActionName)))
            {
                customActionId = customAction.Id;
            }

            return customActionId;
        }

        /// <summary>
        /// Removes a custom action from a web
        /// </summary>
        /// <param name="web">The SharePoint web</param>
        /// <param name="actionName">The ID for the custom action</param>
        public static void DeleteCustomAction(this SPWeb web, string actionName)
        {
            Guid customActionId = GetCustomActionIdForName(web, actionName);

            if (customActionId != default(Guid))
            {
                SPUserCustomAction customAction = web.UserCustomActions[customActionId];
                customAction.Delete();
                web.Update();
            }
        }
    }
}
