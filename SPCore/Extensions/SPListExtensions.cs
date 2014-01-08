using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using SPCore.Caml;
using SPCore.Caml.Clauses;
using SPCore.Helper;

namespace SPCore
{
    public static class SPListExtensions
    {
        /// <summary>
        /// Gets the Url for the list
        /// </summary>
        /// <param name="list">The SPList object</param>
        /// <returns>A list to the Url.</returns>
        public static string GetUrl(this SPList list)
        {
            return SPUrlUtility.CombineUrl(list.ParentWeb.Url, list.RootFolder.Url);
        }

        public static string GetDisplayFormUrl(this SPList list, uint itemId, bool fullUrl = false)
        {
            return string.Format("{0}?ID={1}",
                                 fullUrl
                                     ? list.ParentWeb.Site.MakeFullUrl(list.DefaultDisplayFormUrl)
                                     : list.DefaultDisplayFormUrl, itemId);
        }

        public static string GetEditFormUrl(this SPList list, uint itemId, bool fullUrl = false)
        {
            return string.Format("{0}?ID={1}",
                                 fullUrl
                                     ? list.ParentWeb.Site.MakeFullUrl(list.DefaultEditFormUrl)
                                     : list.DefaultEditFormUrl, itemId);
        }

        public static void ProcessItems(this SPList list, SPQuery query, Action<SPListItemCollection> itemAction, Func<SPListItemCollection, Exception, bool> itemActionOnError)
        {
            if (query.RowLimit == 0)
            {
                query.RowLimit = SPHelper.MaxRowLimit;
            }

            ContentIterator cItems = new ContentIterator();
            //query.Query = query.Query + ContentIterator.ItemEnumerationOrderByNVPField;
            cItems.ProcessListItems(list, query, item => { if (itemAction != null) itemAction(item); }, (item, ex) => itemActionOnError != null && itemActionOnError(item, ex));
        }

        public static SPContentType GetDefaultContentType(this SPList list)
        {
            return list.RootFolder.ContentTypeOrder.Count > 0 ? list.RootFolder.ContentTypeOrder[0] : null;
        }

        public static bool ContainsContentTypeWithId(this SPList list, SPContentTypeId id)
        {
            SPContentType ct = list.GetContentTypeById(id);
            return (ct != null);
        }

        public static SPContentType GetContentTypeById(this SPList list, SPContentTypeId id)
        {
            SPContentType ct = null;
            SPContentTypeId matchId = list.ContentTypes.BestMatch(id);

            if (matchId.IsChildOf(id))
            {
                ct = list.ContentTypes[matchId];
            }

            return ct;
        }

        public static void AddContentType(this SPList list, SPContentTypeId contentTypeId)
        {
            SPContentType contentType = list.ParentWeb.AvailableContentTypes[contentTypeId];
            if (contentType != null) list.AddContentType(contentType);
        }

        public static void AddContentType(this SPList list, SPContentType contentType, bool throwOnNotAllowed = false)
        {
            // Make sure the list accepts content types.
            list.ContentTypesEnabled = true;

            try
            {
                // Add the content type to the list.
                if (!list.IsContentTypeAllowed(contentType))
                    throw new SPException(string.Format("The \"{0}\" content type is not allowed on the \"{1}\" list.",
                                                        contentType.Name, list.Title));
                if (list.ContainsContentTypeWithId(contentType.Id))
                {
                    throw new SPException(string.Format("The content type \"{0}\" is already in use on the \"{1}\" list",
                                                        contentType.Name, list.Title));
                }
            }
            catch (SPException)
            {
                if (throwOnNotAllowed)
                {
                    throw;
                }

                return;
            }

            list.ContentTypes.Add(contentType);
        }

        /// <summary>
        /// Ensures the SPContentType is in the collection. If not, it will be created and added.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentTypeId">The content type id.</param>
        /// <param name="contentTypeName">Name of the content type.</param>
        /// <param name="action"></param>
        /// <returns><c>True</c> if it was added, else <c>False</c>.</returns>
        /// <exception cref="System.ArgumentNullException">For any null parameter.</exception>
        public static SPContentType CreateContentType(this SPList list, SPContentTypeId contentTypeId, string contentTypeName, Action<SPContentType> action = null)
        {
            if (string.IsNullOrEmpty(contentTypeName))
            {
                throw new ArgumentNullException("contentTypeName");
            }

            SPContentType contentType = new SPContentType(contentTypeId, list.ContentTypes, contentTypeName);
            AddContentType(list, contentType, true);

            if (action != null)
            {
                action(contentType);
            }

            contentType.Update(false);
            return contentType;
        }

        public static void Clear(this SPList list, string query = "", uint batchSize = SPHelper.MaxRowLimit, bool recycleBinDisabled = false)
        {
            SPQuery spQuery = new SPQuery
                                  {
                                      Query = query ?? string.Empty,
                                      ViewFields = string.Empty,
                                      ViewFieldsOnly = true,
                                      RowLimit = batchSize
                                  };

            Clear(list, spQuery, recycleBinDisabled);
        }

        public static void Clear(this SPList list, SPQuery spQuery, bool recycleBinDisabled = false)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            int itemCount = list.ItemCount;

            if (itemCount == 0) { return; }

            SPListItemCollectionPosition listItemCollectionPosition;

            do
            {
                SPListItemCollection items = list.GetItems(spQuery.InScope(SPViewScope.Recursive));
                listItemCollectionPosition = items.ListItemCollectionPosition;

                if (list.ItemCount != 0)
                {
                    Clear(items, recycleBinDisabled);
                }

                itemCount -= (int)spQuery.RowLimit;

                if (itemCount <= 0) { break; }

            } while (listItemCollectionPosition != null);

            if (list.ItemCount != 0)
            {
                list.Update();
            }
        }

        public static void Clear(SPListItemCollection items, bool recycleBinDisabled)
        {
            if (items == null || items.Count == 0) { return; }

            int code = 0;

            List<BatchDataMethod> delMethods = new List<BatchDataMethod>(items.Count);
            delMethods.AddRange(from SPListItem item in items
                                select new BatchDataMethod()
                                           {
                                               ListId = items.List.ID,
                                               Id = Convert.ToString(++code) + ",Delete",
                                               Command = BatchDataCommandType.Delete,
                                               ItemId = item.ID,
                                               FileRef =
                                                   (item.File != null && item.File.Exists)
                                                       ? item.File.ServerRelativeUrl
                                                       : string.Empty
                                           });

            string batchCommand = ProcessBatchDataHelper.GetBatch(delMethods, OnErrorAction.Continue);
            // process batch command
            string result;
            SPWeb web = items.List.ParentWeb;

            if (recycleBinDisabled)
            {
                RunWithRecycleBinDisabled(web, () => result = web.ProcessBatchData(batchCommand));
            }
            else
            {
                result = web.ProcessBatchData(batchCommand);
            }
        }

        private static void RunWithRecycleBinDisabled(SPWeb web, Action action)
        {
            bool recycleBinEnabled = web.Site.WebApplication.RecycleBinEnabled;
            SPSite site = web.Site;

            try
            {
                site.WebApplication.RecycleBinEnabled = false;
                action.Invoke();
            }
            finally
            {
                site.WebApplication.RecycleBinEnabled = recycleBinEnabled;
            }
        }

        public static SPWorkflowAssociation CreateWorkflowAssociation(this SPList list,
            SPWorkflowTemplate workflowTemplate, string assocName,
            SPList workflowTasksList, SPList workflowHistoryList,
            bool allowManual, bool autoStartCreate, bool autoStartChange, bool overwrite = false)
        {
            SPWorkflowAssociation workflowAssociation =
                list.CreateWorkflowAssociation(workflowTemplate, assocName, workflowTasksList, workflowHistoryList,
                                               overwrite,
                                               assoc =>
                                               {
                                                   assoc.AllowManual = allowManual;
                                                   assoc.AutoStartCreate = autoStartCreate;
                                                   assoc.AutoStartChange = autoStartChange;
                                               });
            return workflowAssociation;
        }

        public static SPWorkflowAssociation CreateWorkflowAssociation(this SPList list, SPWorkflowTemplate workflowTemplate, string assocName, SPList workflowTasksList, SPList workflowHistoryList, bool overwrite = false, Action<SPWorkflowAssociation> action = null)
        {
            if (workflowTemplate == null) throw new ArgumentNullException("workflowTemplate");

            SPWorkflowAssociation workflowAssociation = null;
            // create the association
            SPWorkflowAssociation assoc =
                SPWorkflowAssociation.CreateListAssociation(workflowTemplate, assocName, workflowTasksList,
                                                            workflowHistoryList);

            if (action != null)
            {
                action(assoc);
            }

            //apply the association to the list
            if (list != null)
            {
                if (overwrite)
                {
                    SPWorkflowAssociation dupWfAssoc =
                        list.WorkflowAssociations.GetAssociationByName(assocName, list.ParentWeb.Locale);

                    if (dupWfAssoc != null)
                    {
                        if (dupWfAssoc.BaseId == workflowTemplate.Id)
                        {
                            list.WorkflowAssociations.Remove(dupWfAssoc);
                        }
                        else
                        {
                            throw new SPException(string.Format("Duplicate workflow name \"{0}\" is detected.", assocName));
                        }
                    }
                }

                workflowAssociation = list.WorkflowAssociations.Add(assoc);
            }

            return workflowAssociation;
        }

        public static SPFolder AddFolder(this SPList list, string folderName, bool ignoreIfExists = false, string parentFolderUrl = "")
        {
            if (parentFolderUrl.StartsWith(list.RootFolder.ServerRelativeUrl))
            {
                parentFolderUrl = parentFolderUrl.Substring(list.RootFolder.ServerRelativeUrl.Length);
            }

            if (ignoreIfExists)
            {
                string folderUrl = SPUtility.ConcatUrls(list.RootFolder.ServerRelativeUrl, SPUtility.ConcatUrls(parentFolderUrl, folderName));

                SPFolder existingFolder = list.ParentWeb.GetFolder(folderUrl);
                if (existingFolder.Exists)
                {
                    return existingFolder;
                }
            }

            SPListItem item = list.AddItem(SPUtility.ConcatUrls(list.RootFolder.ServerRelativeUrl, parentFolderUrl),
                                             SPFileSystemObjectType.Folder, folderName);
            item.Update();
            return item.Folder;
        }

        public static SPView GetView(this SPList list, string viewName)
        {
            SPView view =
                list.Views.Cast<SPView>().FirstOrDefault(
                    v => v.Title.Equals(viewName, StringComparison.InvariantCultureIgnoreCase));
            return view;
        }

        public static void RunAsSystem(this SPList list, Action<SPList> action)
        {
            list.ParentWeb.RunAsSystem(web => { if (action != null) action.Invoke(web.Lists[list.ID]); });
        }

        public static void RunAsUser(this SPList list, SPUser user, Action<SPList> action)
        {
            list.ParentWeb.RunAsUser(user, web => { if (action != null) action.Invoke(web.Lists[list.ID]); });
        }

        public static IEnumerable<SPListItem> GetItemsByContentType(this SPList list, SPContentType contentType, bool includeChildren = false, SPQuery query = null)
        {
            string conditionQuery =
                string.Format(
                    "<Where><BeginsWith><FieldRef Name=\"ContentTypeId\" /><Value Type=\"ContentTypeId\">{0}</Value></BeginsWith></Where>",
                    contentType.Id);

            Where where = new Where(conditionQuery);

            if (query == null)
            {
                query = new Query() { Where = where }.ToSPQuery().InScope(SPViewScope.Recursive);
            }
            else
            {
                Query q = Query.GetFromSPQuery(query);
                q.Where = q.Where != null ? Where.Combine(q.Where, where) : where;
                query.Query = q.ToString(false);
            }

            SPListItemCollection items = list.GetItems(query);

            foreach (SPListItem item in items)
            {
                if (includeChildren)
                {
                    //if (item.ContentTypeId.IsChildOf(contentType.Id) || item.ContentTypeId.Equals(contentType.Id))
                    //{
                    yield return item;
                    //}
                }
                else
                {
                    if (item.ContentType.Name.Equals(contentType.Name))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
