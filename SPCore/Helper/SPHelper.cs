using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Utilities;

namespace SPCore.Helper
{
    public static class SPHelper
    {
        public const uint MaxRowLimit = 2000;

        /// <summary>
        /// This function converts strings in Hebrew (it can be used for other
        /// languages too) to the hexadecimal presentation that is used by SharePoint
        /// for internal field names
        /// </summary>
        /// <param name="inputString">The string to convert</param>
        /// <returns>The converted string</returns>
        public static string ConvertHebrewToUnicodeHex(string inputString)
        {
            StringBuilder outputString = new StringBuilder();
            //convert the string to char array and manipulate the chars
            char[] charArray = inputString.ToCharArray();

            foreach (char c in charArray)
            {
                int charIntRepresentation = c;
                outputString.Append("_x" + String.Format("{0:x4}", Convert.ToUInt32(charIntRepresentation.ToString())) + "_");

            }
            return outputString.ToString();
        }

        /// <summary>
        ///This function converts strings in the unicode hexadecimal presentation
        /// for Hebrew that is used by SharePoint to a string in Hebrew
        /// </summary>
        /// <param name="inputString">The string to convert</param>
        /// <returns>The converted string</returns>
        public static string ConvertUnicodeHexToString(string inputString)
        {
            StringBuilder outputString = new StringBuilder();
            //Each char is represented in the following format
            // _x????_ (7 chars long) where ???? is number in hex 

            for (int i = 0; i < inputString.Length; i += 7)
            {
                string hexValue = inputString.Substring(i, 7).Substring(2, 4);
                char charCode = (char)UInt32.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                outputString.Append(charCode);
            }
            return outputString.ToString();
        }

        public static bool IsGuid(string guid)
        {
            try
            {
                SPUtility.ObjectToGuid(guid);
                return true;
            }
            catch (FormatException) { }

            return false;
        }

        /// <summary>
        /// Removes characters from a string that can't exist in an URL.
        /// </summary>
        /// <param name="input">The string to remove illegal characters from</param>
        /// <returns></returns>
        public static string RemoveIllegalUrlCharacters(string input)
        {
            input = input.Trim();
            int index = SPUrlUtility.IndexOfIllegalCharInUrlLeafName(input);
            while (index >= 0)
            {
                input = input.Remove(index, 1);
                index = SPUrlUtility.IndexOfIllegalCharInUrlLeafName(input);
            }

            return input;
        }

        internal static T GetPropertyValue<T>(this object obj, string propName)
        {
            var type = obj.GetType();

            var prop = type.GetProperty(propName);

            var val = prop.GetValue(obj, null);

            return val is T
                ? (T)val
                : default(T);
        }

        public static void RunAsSystem<TS>(string url, Action<TS> action)
          where TS : SPSite
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite elevSite = new SPSite(url, SPUserToken.SystemAccount))
                {
                    action(elevSite as TS);
                }
            });
        }

        public static void RunAsUser<TS>(string url, SPUser user, Action<TS> action)
         where TS : SPSite
        {
            using (SPSite elevSite = new SPSite(url, user.UserToken))
            {
                action(elevSite as TS);
            }
        }

        public static void RunAsSystem(string url, Action<SPSite, SPWeb> action)
        {
            RunAsSystem<SPSite>(url, elevSite =>
            {
                using (SPWeb elevWeb = elevSite.OpenWeb())
                {
                    action(elevSite, elevWeb);
                }
            });
        }

        public static void RunAsUser(string url, SPUser user, Action<SPSite, SPWeb> action)
        {
            RunAsUser<SPSite>(url, user, elevSite =>
            {
                using (SPWeb elevWeb = elevSite.OpenWeb())
                {
                    action(elevSite, elevWeb);
                }
            });
        }

        public static void CopyFiles(SPDocumentLibrary sourceDocLib, SPDocumentLibrary targetDocLib,
            bool overwrite = false, bool includeHistory = false, string queryStr = "", string targetContentTypeName = "")
        {
            SPQuery query = new SPQuery { RowLimit = MaxRowLimit, Folder = sourceDocLib.RootFolder, Query = queryStr };

            SPListItemCollectionPosition listItemCollectionPosition;

            do
            {
                SPListItemCollection items = sourceDocLib.GetItems(query.GetView(SPViewScope.FilesOnly));
                listItemCollectionPosition = items.ListItemCollectionPosition;

                foreach (SPListItem item in items)
                {
                    SPFile file = item.File;
                    SPFolder targetFolder = targetDocLib.RootFolder;

                    if (!file.Exists) { continue; }

                    SPFile newFile = includeHistory
                                         ? CopyFileAndHistory(file, targetFolder, overwrite)
                                         : CopyFile(file, targetFolder, overwrite);

                    SPListItem newItem = newFile.Item;

                    newItem[SPBuiltInFieldId.ContentType] = string.IsNullOrEmpty(targetContentTypeName)
                                                                ? item.ContentType.Name
                                                                : targetContentTypeName;

                    newItem[SPBuiltInFieldId.Created] = file.TimeCreated;
                    //newItem[SPBuiltInFieldId.Modified] = file.TimeLastModified;

                    newItem[SPBuiltInFieldId.Author] = newFile.Author;
                    //newItem[SPBuiltInFieldId.Editor] = newFile.ModifiedBy;
                    newFile.Item.SystemUpdate(false);
                }

            } while (listItemCollectionPosition != null);
        }

        public static void CopyFile(string sourceFileUrl, string targetFolderUrl,
            bool overwrite = false, bool includeHistory = false, bool runAsSystem = false)
        {
            SPUserToken userToken = runAsSystem
                                        ? SPUserToken.SystemAccount
                                        : SPContext.Current.Web.CurrentUser.UserToken;

            using (SPSite sourceSite = new SPSite(sourceFileUrl, userToken))
            using (SPWeb sourceWeb = sourceSite.OpenWeb())
            using (SPSite targetSite = new SPSite(targetFolderUrl, userToken))
            using (SPWeb targetWeb = targetSite.OpenWeb())
            {
                sourceWeb.DoUnsafeUpdate(() => targetWeb.DoUnsafeUpdate(() =>
                {
                    SPFile sourceFile = sourceWeb.GetFile(sourceFileUrl);
                    SPFolder targetFolder = targetWeb.GetFolder(targetFolderUrl);

                    if (includeHistory)
                    {
                        CopyFileAndHistory(sourceFile, targetFolder, overwrite);
                    }
                    else
                    {
                        CopyFile(sourceFile, targetFolder, overwrite);
                    }
                }));
            }
        }

        public static string CopyFile(SPSite site, string sourceFileUrl, string targetFolderUrl, bool overwrite = false, bool includeHistory = false)
        {
            SPWeb sourceWeb = null;
            SPWeb targetWeb = null;

            try
            {
                sourceWeb = site.OpenWeb(sourceFileUrl, false);

                if (!sourceWeb.Exists)
                {
                    throw new SPException(string.Format("Source file with URL '{0}' is not found.", sourceFileUrl));
                }

                targetWeb = site.OpenWeb(targetFolderUrl, false);

                if (!targetWeb.Exists)
                {
                    throw new SPException(string.Format("Target folder with URL '{0}' is not found.", targetFolderUrl));
                }

                SPFile sourceFile = sourceWeb.GetFile(sourceFileUrl);
                SPFolder targetFolder = targetWeb.GetFolder(targetFolderUrl);

                if (includeHistory)
                {
                    return CopyFileAndHistory(sourceFile, targetFolder, overwrite).ServerRelativeUrl;
                }
                else
                {
                    if (targetWeb.Url == sourceWeb.Url)
                    {
                        string newFileUrl = SPUrlUtility.CombineUrl(targetFolder.ServerRelativeUrl, sourceFile.Name);
                        sourceFile.CopyTo(newFileUrl, overwrite);
                        return newFileUrl;
                    }
                    else
                    {
                        return CopyFile(sourceFile, targetFolder, overwrite).ServerRelativeUrl;
                    }
                }
            }
            finally
            {
                if (sourceWeb != null)
                {
                    sourceWeb.Dispose();
                }
                if (targetWeb != null)
                {
                    targetWeb.Dispose();
                }
            }
        }

        public static SPFile CopyFile(SPFile sourceFile, SPFolder targetFolder, bool overwrite = false)
        {
            SPFile newFile;

            using (Stream stream = sourceFile.OpenBinaryStream())
            {
                string fileName = SPUrlUtility.CombineUrl(targetFolder.ServerRelativeUrl, sourceFile.Name);

                bool crossSite = (sourceFile.Web.Site.ID != targetFolder.ParentWeb.Site.ID);

                SPUser author;
                SPUser editor;

                if (crossSite)
                {
                    author = targetFolder.ParentWeb.EnsureUser(sourceFile.Author.LoginName);
                    editor = targetFolder.ParentWeb.EnsureUser(sourceFile.ModifiedBy.LoginName);
                }
                else
                {
                    author = sourceFile.Author;
                    editor = sourceFile.ModifiedBy;
                }

                newFile = targetFolder.Files.Add(
                    fileName, stream,
                    sourceFile.Properties, author, editor, sourceFile.TimeCreated,
                    sourceFile.TimeLastModified, sourceFile.CheckInComment, overwrite);
            }

            return newFile;
        }

        public static SPFile CopyFileAndHistory(SPFile sourceFile, SPFolder targetFolder, bool overwrite = false)
        {
            SPFile targetFile =
                targetFolder.ParentWeb.GetFile(SPUrlUtility.CombineUrl(targetFolder.ServerRelativeUrl, sourceFile.Name));

            if (targetFile.Exists)
            {
                if (overwrite)
                {
                    if (targetFile.RequiresCheckout)
                    {
                        if (targetFile.CheckOutType == SPFile.SPCheckOutType.None)
                        {
                            targetFile.CheckOut();
                        }
                        else
                        {
                            targetFile.UndoCheckOut();
                        }
                    }

                    targetFile.Delete();
                }
                else
                {
                    return targetFile;
                }
            }

            int countVersions = sourceFile.Versions.Count;

            for (int i = 0; i < countVersions; i++)
            {
                SPFileVersion sourceFileVer = sourceFile.Versions[i];

                if (!sourceFileVer.IsCurrentVersion)
                {
                    SPFile historyFile;

                    using (Stream stream = sourceFileVer.OpenBinaryStream())
                    {
                        string fileName = SPUrlUtility.CombineUrl(targetFolder.ServerRelativeUrl,
                                                                  sourceFileVer.File.Name);

                        bool crossSite = (sourceFile.Web.Site.ID != targetFolder.ParentWeb.Site.ID);

                        SPUser author;
                        SPUser editor;

                        if (crossSite)
                        {
                            author = targetFolder.ParentWeb.EnsureUser(sourceFileVer.CreatedBy.LoginName);
                            editor = targetFolder.ParentWeb.EnsureUser(sourceFileVer.CreatedBy.LoginName);
                        }
                        else
                        {
                            author = sourceFileVer.CreatedBy;
                            editor = sourceFileVer.CreatedBy;
                        }

                        historyFile = targetFolder.Files.Add(
                            fileName, stream, sourceFileVer.Properties, author,
                            editor, sourceFileVer.Created, sourceFileVer.Created,
                            sourceFileVer.CheckInComment, true);
                    }
                }
            }

            return CopyFile(sourceFile, targetFolder, true);
        }

        public static DataTable GetEmptyDataTable(SPList list, params Guid[] excludedFieldIds)
        {
            DataTable dt = new DataTable(list.Title);

            SPField fieldId = list.Fields[SPBuiltInFieldId.ID];

            dt.Columns.Add(new DataColumn
            {
                ColumnName = fieldId.InternalName,
                DataType = typeof(int),
                Caption = fieldId.Title,
                ReadOnly = true
            });

            foreach (SPField field in list.Fields)
            {
                if (field.Hidden) { continue; }

                if ((excludedFieldIds != null && !excludedFieldIds.Contains(field.Id)) && (SPBuiltInFieldId.Contains(field.Id) && field.ReadOnlyField))
                {
                    continue;
                }

                if (field.Type == SPFieldType.Calculated ||
                    field.Type == SPFieldType.Attachments ||
                    field.Type == SPFieldType.Counter ||
                    field.Type == SPFieldType.WorkflowStatus)
                {
                    continue;
                }

                if (dt.Columns.Contains(field.InternalName)) { continue; }

                dt.Columns.Add(new DataColumn
                {
                    ColumnName = field.InternalName,
                    DataType = field.FieldValueType ?? typeof(string),
                    Caption = field.Title
                });
            }

            return dt;
        }

        public static void BatchDataTableToList(DataTable dt, SPList list, OnErrorAction onError)
        {
            if (dt == null || dt.Rows.Count == 0) { return; }

            List<BatchDataMethod> methods = new List<BatchDataMethod>();

            bool idColExists = dt.Columns.Contains("ID");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];

                BatchDataMethod method = new BatchDataMethod()
                {
                    ListId = list.ID
                };

                if (idColExists && !Convert.IsDBNull(dr["ID"]))
                {
                    method.Id = Convert.ToString(i) + ",Update";
                    method.Command = BatchDataCommandType.Update;
                    method.ItemId = Convert.ToInt32(dr["ID"]);
                }
                else
                {
                    method.Id = Convert.ToString(i) + ",Add";
                    method.Command = BatchDataCommandType.Add;
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ReadOnly || col.ColumnName == "ID") { continue; }

                    method.ColumnsData.Add(new BatchDataColumn(col.ColumnName, dr[col.ColumnName]));
                }

                methods.Add(method);
            }

            string batchCommand = ProcessBatchDataHelper.GetBatch(methods, onError);

            string result;
            SPWeb web = list.ParentWeb;
            result = web.ProcessBatchData(batchCommand);
            //web.DoUnsafeUpdate(w => result = w.ProcessBatchData(batchCommand));
        }

        public static void AddDataTableToList(DataTable dt, SPList list, bool systemUpdate = false, bool doNotFireEvents = false)
        {
            if (dt == null) return;

            foreach (DataRow dr in dt.Rows)
            {
                SPListItem item = list.AddItem();

                string[] attachments = null;

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ReadOnly) { continue; }

                    if (col.DataType == typeof(TaxonomyFieldValue))
                    {
                        TaxonomyField taxonomyField = (TaxonomyField)list.Fields.GetField(col.ColumnName);
                        taxonomyField.SetFieldValue(item, (TaxonomyFieldValue)(Convert.IsDBNull(dr[col]) ? new TaxonomyFieldValue(taxonomyField) : dr[col]));
                    }
                    else if (col.DataType == typeof(TaxonomyFieldValueCollection))
                    {
                        TaxonomyField taxonomyField = (TaxonomyField)list.Fields.GetField(col.ColumnName);
                        taxonomyField.SetFieldValue(item, (TaxonomyFieldValueCollection)(Convert.IsDBNull(dr[col]) ? new TaxonomyFieldValueCollection(taxonomyField) : dr[col]));
                    }
                    else if (col.ColumnName == "Attachments")
                    {
                        if (!Convert.IsDBNull(dr[col]))
                        {
                            attachments = dr[col].ToString().Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        }
                    }
                    else
                    {
                        item[col.ColumnName] = Convert.IsDBNull(dr[col]) ? null : dr[col];
                    }
                }

                if (attachments != null)
                {
                    foreach (string attachment in attachments)
                    {
                        if (!string.IsNullOrEmpty(attachment))
                        {
                            item.AttachFile(attachment.Trim(), false, true);
                        }
                    }
                }

                if (systemUpdate)
                {
                    if (doNotFireEvents)
                    {
                        RunWithDisabledEvent(item.SystemUpdate);
                    }

                    item.SystemUpdate();
                }
                else
                {
                    if (doNotFireEvents)
                    {
                        RunWithDisabledEvent(item.Update);
                    }

                    item.Update();
                }
            }
        }

        public static void RunAction(Action<SPWeb> action, SPSite site, ActionScope scope)
        {
            if (action == null) { return; }

            switch (scope)
            {
                case ActionScope.Web:
                    using (SPWeb web = site.OpenWeb())
                    {
                        action(web);
                    }
                    break;
                case ActionScope.RootWeb:
                    action(site.RootWeb);
                    break;
                case ActionScope.SubWebs:
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.Webs.AsSafeEnumerable().ForEach(action);
                    }
                    break;
                case ActionScope.SubWebsRecursion:
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.Webs.AsSafeEnumerable().RecursiveSelect(w => w.Webs.AsSafeEnumerable()).ForEach(action);
                    }
                    break;
                case ActionScope.WebAndSubWebs:
                    using (SPWeb web = site.OpenWeb())
                    {
                        action(web);
                        web.Webs.AsSafeEnumerable().ForEach(action);
                    }
                    break;
                case ActionScope.WebAndSubWebsRecursion:
                    using (SPWeb web = site.OpenWeb())
                    {
                        action(web);
                        web.Webs.AsSafeEnumerable().RecursiveSelect(w => w.Webs.AsSafeEnumerable()).ForEach(action);
                    }
                    break;
                case ActionScope.AllWebs:
                    site.AllWebs.AsSafeEnumerable().ForEach(action);
                    break;
            }
        }

        public static void RunAction(Action<SPWeb> action, string url, ActionScope scope, SPUser user = null)
        {
            if (user == null)
            {
                RunAsSystem<SPSite>(url, site => RunAction(action, site, scope));
            }
            else
            {
                RunAsUser<SPSite>(url, user, site => RunAction(action, site, scope));
            }
        }

        public static void RunWithDisabledEvent(Action action)
        {
            using (EventsFiringDisabledScope scope = new EventsFiringDisabledScope())
            {
                action.Invoke(); // will NOT fire events
            }
        }

        public static IEnumerable<string> GetSiteUrls(SPWebApplication webApplication)
        {
            foreach (string sc in webApplication.Sites.Names)
            {
                yield return
                    string.IsNullOrEmpty(sc)
                        ? SPUrlUtility.CombineUrl(webApplication.GetResponseUri(SPUrlZone.Default).AbsoluteUri, "/")
                        : SPUrlUtility.CombineUrl(webApplication.GetResponseUri(SPUrlZone.Default).AbsoluteUri, sc);
            }
        }

        public static IEnumerable<SPList> GetListsByContentType(SPWeb web, SPContentType contentType)
        {
            return GetListsByContentType<SPList>(web, contentType);
        }

        public static IEnumerable<TList> GetListsByContentType<TList>(SPWeb web, SPContentType contentType)
            where TList : SPList
        {
            if (contentType == null)
            {
                return new List<TList>();
            }

            IList<SPContentTypeUsage> usages = SPContentTypeUsage.GetUsages(contentType);
            return GetListsByContentType<TList>(web, usages);
        }

        public static IEnumerable<TList> GetListsByContentType<TList>(SPWeb web, IList<SPContentTypeUsage> usages)
           where TList : SPList
        {
            List<string> listUrls = GetListUrls(web, usages);
            return listUrls.Count == 0 ? new List<TList>() : GetListsByUrls<TList>(web, listUrls);
        }

        public static List<string> GetListUrls(SPWeb web, IList<SPContentTypeUsage> usages)
        {
            List<string> listUrls = new List<string>();

            if (usages != null && usages.Count > 0)
            {
                IEnumerable<string> subWebUrls = web.GetSubWebUrls();

                foreach (SPContentTypeUsage usage in usages.Where(usage => usage.IsUrlToList && !listUrls.Contains(usage.Url)))
                {
                    if (usage.Url.StartsWith(web.ServerRelativeUrl.TrimEnd('/') + "/"))
                    {
                        if (subWebUrls.Any(subWebUrl => usage.Url.StartsWith(subWebUrl)))
                        {
                            continue;
                        }

                        listUrls.Add(usage.Url);
                    }
                }
            }

            return listUrls;
        }

        public static Dictionary<string, List<string>> GetListUrls(SPContentType contentType)
        {
            IList<SPContentTypeUsage> usages = SPContentTypeUsage.GetUsages(contentType);
            Dictionary<string, List<string>> listUrls = new Dictionary<string, List<string>>();

            if (usages != null && usages.Count > 0)
            {
                foreach (SPContentTypeUsage usage in usages.Where(usage => usage.IsUrlToList))
                {
                    if (!usage.IsUrlToList) continue;

                    Match m = Regex.Match(usage.Url, "^(?<web>.*?/)(Lists/)?(?<list>[^/]+)$");

                    if (m.Success)
                    {
                        string webUrl = m.Groups["web"].Value;

                        if (listUrls.ContainsKey(webUrl))
                        {
                            listUrls[webUrl].Add(m.Groups["list"].Value);
                        }
                        else
                        {
                            listUrls.Add(webUrl, new List<string>()
                                                     {
                                                         m.Groups["list"].Value
                                                     });
                        }
                    }
                }
            }

            return listUrls;
        }

        public static IEnumerable<TList> GetListsByUrls<TList>(SPWeb web, IEnumerable<string> listUrls)
            where TList : SPList
        {
            if (listUrls != null)
                foreach (string listUrl in listUrls)
                {
                    SPList list = web.TryGetListByUrl(listUrl);

                    if (list != null)
                    {
                        if (list is TList)
                        {
                            yield return (TList)list;
                        }
                    }
                }
        }
    }
}
