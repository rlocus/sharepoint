using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.BusinessData.Infrastructure;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Utilities;
using SPCore.Helper;
using SPCore.Taxonomy;

namespace SPCore
{
    public static class SPListItemExtensions
    {
        #region [ Private methods ]

        private static SPFieldUserValue GetUserValue(object value)
        {
            SPFieldUserValue ret;

            if (value != null)
            {
                if (value is SPFieldUserValue)
                {
                    ret = value as SPFieldUserValue;
                }
                else
                {
                    ret = value is SPFieldUserValueCollection
                              ? ((value as SPFieldUserValueCollection).FirstOrDefault()
                                 ?? new SPFieldUserValue())
                              : new SPFieldUserValue();
                }
            }
            else
            {
                ret = new SPFieldUserValue();
            }

            return ret;
        }

        private static SPUser GetUser(object value)
        {
            return GetUserValue(value).User;
        }

        private static SPFieldUserValueCollection GetUserValues(object value)
        {
            SPFieldUserValueCollection ret;

            if (value != null)
            {
                ret = value is SPFieldUserValueCollection
                          ? value as SPFieldUserValueCollection
                          : (value is SPFieldUserValue
                                 ? new SPFieldUserValueCollection { value as SPFieldUserValue }
                                 : new SPFieldUserValueCollection());
            }
            else
            {
                ret = new SPFieldUserValueCollection();
            }

            return ret;
        }

        private static IEnumerable<SPUser> GetUsers(object value)
        {
            return GetUserValues(value).Select(userValue => userValue.User);
        }

        //private static SPFieldLookupValue GetLookupValue(object value)
        //{
        //    SPFieldLookupValue ret;

        //    if (value != null)
        //    {
        //        if (value is SPFieldLookupValue)
        //        {
        //            ret = value as SPFieldLookupValue;
        //        }
        //        else
        //        {
        //            ret = new SPFieldLookupValueCollection(value.ToString()).FirstOrDefault()
        //                  ?? new SPFieldLookupValue();
        //        }
        //    }
        //    else
        //    {
        //        ret = new SPFieldLookupValue();
        //    }

        //    return ret;
        //}

        //private static string GetLookupString(object value)
        //{
        //    return GetLookupValue(value).LookupValue;
        //}

        //private static SPFieldLookupValueCollection GetLookupValues(object value)
        //{
        //    SPFieldLookupValueCollection ret;

        //    if (value != null)
        //    {
        //        if (value is SPFieldLookupValue)
        //        {
        //            ret = new SPFieldLookupValueCollection { value as SPFieldLookupValue };
        //        }
        //        else
        //        {
        //            ret = new SPFieldLookupValueCollection(value.ToString());
        //        }
        //    }
        //    else
        //    {
        //        ret = new SPFieldLookupValueCollection();
        //    }

        //    return ret;
        //}

        //private static string[] GetLookupArrayString(object value)
        //{
        //    return GetLookupValues(value).Select(lookupValue => lookupValue.LookupValue).ToArray();
        //}

        //private static string GetUrl(object value)
        //{
        //    if (value != null)
        //    {
        //        if (value is SPFieldUrlValue)
        //        {
        //            return (value as SPFieldUrlValue).Url;
        //        }
        //    }

        //    return new SPFieldUrlValue().Url;
        //}

        //private static int GetRating(object value)
        //{
        //    if (value != null)
        //    {
        //        if (value is SPFieldRatingScaleValue)
        //        {
        //            return (value as SPFieldRatingScaleValue).Count;
        //        }
        //    }

        //    return new SPFieldRatingScaleValue().Count;
        //}

        #endregion

        #region [ Public methods ]

        public static object TryGetValue(this SPListItem item, string fieldName)
        {
            try
            {
                return GetValue(item, fieldName);
            }
            catch (ArgumentException)
            {
                throw;
            }
            //TODO: define the type of exception
            catch
            {
                return null;
            }
        }

        public static object GetValue(this SPListItem item, string fieldName)
        {
            SPField field = item.Fields.GetField(fieldName);
            object value = item[field.Id];
            return value != null ? field.GetFieldValue(value.ToString()) : null;
        }

        public static object TryGetValue(this SPListItem item, Guid fieldId)
        {
            try
            {
                return GetValue(item, fieldId);
            }
            catch (ArgumentException)
            {
                throw;
            }
            //TODO: define the type of exception
            catch
            {
                return null;
            }
        }

        public static object GetValue(this SPListItem item, Guid fieldId)
        {
            SPField field = item.Fields[fieldId];
            object value = item[field.Id];
            return value != null ? field.GetFieldValue(value.ToString()) : null;
        }

        public static bool TryGetValue<T>(this SPListItem item, string fieldName, out T value)
        {
            try
            {
                value = GetValue<T>(item, fieldName);
                return true;
            }
            catch (ArgumentException)
            {
                throw;
            }
            //TODO: define the type of exception
            catch
            {
                value = default(T);
                return false;
            }
        }

        public static T GetValue<T>(this SPListItem item, string fieldName)
        {
            object o = GetValue(item, fieldName);
            return (T)o;
        }

        public static bool TryGetValue<T>(this SPListItem item, Guid fieldId, out T value)
        {
            try
            {
                value = GetValue<T>(item, fieldId);
                return true;
            }
            catch (ArgumentException)
            {
                throw;
            }
            //TODO: define the type of exception
            catch
            {
                value = default(T);
                return false;
            }
        }

        public static T GetValue<T>(this SPListItem item, Guid fieldId)
        {
            object o = GetValue(item, fieldId);
            return (T)o;
        }

        public static object GetValue(this SPListItemVersion itemVersion, string fieldName)
        {
            SPField field = itemVersion.Fields.GetField(fieldName);
            object value = itemVersion[field.InternalName];
            return value != null ? field.GetFieldValue(value.ToString()) : null;
        }

        public static object GetValue(this SPListItemVersion itemVersion, Guid fieldId)
        {
            SPField field = itemVersion.Fields[fieldId];
            object value = itemVersion[field.InternalName];
            return value != null ? field.GetFieldValue(value.ToString()) : null;
        }

        public static T GetValue<T>(this SPListItemVersion itemVersion, string fieldName)
        {
            object o = GetValue(itemVersion, fieldName);
            return (T)o;
        }

        public static T GetValue<T>(this SPListItemVersion itemVersion, Guid fieldId)
        {
            object o = GetValue(itemVersion, fieldId);
            return (T)o;
        }

        public static bool TryGetValue<T>(this SPListItemVersion itemVersion, string fieldName, out T value)
        {
            try
            {
                value = GetValue<T>(itemVersion, fieldName);
                return true;
            }
            catch (ArgumentException)
            {
                throw;
            }
            //TODO: define the type of exception
            catch
            {
                value = default(T);
                return false;
            }
        }

        public static bool TryGetValue<T>(this SPListItemVersion itemVersion, Guid fieldId, out T value)
        {
            try
            {
                value = GetValue<T>(itemVersion, fieldId);
                return true;
            }
            catch (ArgumentException)
            {
                throw;
            }
            //TODO: define the type of exception
            catch
            {
                value = default(T);
                return false;
            }
        }

        //public static string GetLookupValue(this SPListItem item, string fieldName)
        //{
        //    object value = GetValue(item, fieldName);

        //    return GetLookupString(value);
        //}

        //public static string GetLookupValue(this SPListItem item, Guid fieldId)
        //{
        //    object value = GetValue(item, fieldId);

        //    return GetLookupString(value);
        //}

        //public static string[] GetLookupValues(this SPListItem item, string fieldName)
        //{
        //    object value = GetValue(item, fieldName);

        //    return GetLookupArrayString(value);
        //}

        //public static string[] GetLookupValues(this SPListItem item, Guid fieldId)
        //{
        //    object value = GetValue(item, fieldId);

        //    return GetLookupArrayString(value);
        //}

        public static SPUser GetUser(this SPListItem item, Guid fieldId)
        {
            object value = GetValue(item, fieldId);

            return GetUser(value);
        }

        public static SPUser GetUser(this SPListItem item, string fieldName)
        {
            object value = GetValue(item, fieldName);

            return GetUser(value);
        }

        public static IEnumerable<SPUser> GetUsers(this SPListItem item, string fieldName)
        {
            object value = GetValue(item, fieldName);

            return GetUsers(value);
        }

        public static IEnumerable<SPUser> GetUsers(this SPListItem item, Guid fieldId)
        {
            object value = GetValue(item, fieldId);

            return GetUsers(value);
        }

        //public static string GetUrl(this SPListItem item, string fieldName)
        //{
        //    object value = GetValue(item, fieldName);

        //    return GetUrl(value);
        //}

        //public static string GetUrl(this SPListItem item, Guid fieldId)
        //{
        //    object value = GetValue(item, fieldId);

        //    return GetUrl(value);
        //}

        //public static int GetRating(this SPListItem item, string fieldName)
        //{
        //    object value = GetValue(item, fieldName);

        //    return GetRating(value);
        //}

        //public static int GetRating(this SPListItem item, Guid fieldId)
        //{
        //    object value = GetValue(item, fieldId);

        //    return GetRating(value);
        //}

        //public static TaxonomyFieldValue GetTaxonomyValue(this SPListItem item, Guid fieldId)
        //{
        //    var value = item[fieldId];

        //    TaxonomyFieldValue ret = value != null
        //                                ? (TaxonomyFieldControl.GetTaxonomyValue(value.ToString())
        //                                   ?? new TaxonomyFieldValue(string.Empty))
        //                                : new TaxonomyFieldValue(string.Empty);

        //    return ret;
        //}

        //public static TaxonomyFieldValue GetTaxonomyValue(this SPListItem item, string fieldName)
        //{
        //    var value = item[fieldName];

        //    TaxonomyFieldValue ret = value != null
        //                                 ? (TaxonomyFieldControl.GetTaxonomyValue(value.ToString())
        //                                    ?? new TaxonomyFieldValue(string.Empty))
        //                                 : new TaxonomyFieldValue(string.Empty);

        //    return ret;
        //}

        //public static TaxonomyFieldValueCollection GetTaxonomyValues(this SPListItem item, Guid fieldId)
        //{
        //    var value = item[fieldId];

        //    TaxonomyFieldValueCollection ret = value != null
        //                                           ? (TaxonomyFieldControl.GetTaxonomyCollection(value.ToString())
        //                                              ?? new TaxonomyFieldValueCollection(string.Empty))
        //                                           : new TaxonomyFieldValueCollection(string.Empty);

        //    return ret;
        //}

        //public static TaxonomyFieldValueCollection GetTaxonomyValues(this SPListItem item, string fieldName)
        //{
        //    var value = item[fieldName];

        //    TaxonomyFieldValueCollection ret = value != null
        //                                           ? (TaxonomyFieldControl.GetTaxonomyCollection(value.ToString())
        //                                              ?? new TaxonomyFieldValueCollection(string.Empty))
        //                                           : new TaxonomyFieldValueCollection(string.Empty);

        //    return ret;
        //}

        //public static string GetTaxonomyLabel(this SPListItem item, Guid fieldId)
        //{
        //    return GetTaxonomyValue(item, fieldId).Label;
        //}

        //public static string GetTaxonomyLabel(this SPListItem item, string fieldName)
        //{
        //    return GetTaxonomyValue(item, fieldName).Label;
        //}

        //public static string[] GetTaxonomyLabels(this SPListItem item, Guid fieldId)
        //{
        //    return GetTaxonomyValues(item, fieldId).Where(@tax => tax.Label != null).Select(@tax => tax.Label).ToArray();
        //}

        //public static string[] GetTaxonomyLabels(this SPListItem item, string fieldName)
        //{
        //    return GetTaxonomyValues(item, fieldName).Where(@tax => tax.Label != null).Select(@tax => tax.Label).ToArray();
        //}

        public static void SetTaxonomyValue(this SPListItem item, string fieldName, string value, bool addIfDoesNotExist)
        {
            TaxonomyField taxonomyField = (TaxonomyField)item.Fields[fieldName];

            if (taxonomyField.AllowMultipleValues)
            {
                TaxonomyFieldValueCollection taxValue =
                    TaxonomyHelper.GetTaxonomyFieldValues(item.Web.Site, taxonomyField, new[] { value }, addIfDoesNotExist);
                taxonomyField.SetFieldValue(item, taxValue);
            }
            else
            {
                TaxonomyFieldValue taxValue = TaxonomyHelper.GetTaxonomyFieldValue(item.Web.Site, taxonomyField, value, addIfDoesNotExist);
                taxonomyField.SetFieldValue(item, taxValue);
            }
        }

        public static void SetTaxonomyValues(this SPListItem item, string fieldName, bool addIfDoesNotExist, params string[] values)
        {
            TaxonomyField taxonomyField = (TaxonomyField)item.Fields[fieldName];

            if (taxonomyField.AllowMultipleValues)
            {
                TaxonomyFieldValueCollection taxValue = TaxonomyHelper.GetTaxonomyFieldValues(item.Web.Site, taxonomyField, values, addIfDoesNotExist);
                taxonomyField.SetFieldValue(item, taxValue);
            }
            else
            {
                TaxonomyFieldValue taxValue = values != null && values.Length > 0
                                                  ? TaxonomyHelper.GetTaxonomyFieldValue(item.Web.Site, taxonomyField, values[0], addIfDoesNotExist)
                                                  : new TaxonomyFieldValue(taxonomyField);

                taxonomyField.SetFieldValue(item, taxValue);
            }
        }

        public static void AttachFile(this SPListItem item, string filePath, bool overwrite = false, bool updateRequired = false)
        {
            string leafName = Path.GetFileName(filePath);
            item.AttachFile(filePath, leafName, overwrite, updateRequired);
        }

        public static void AttachFile(this SPListItem item, string filePath, string leafName, bool overwrite = false, bool updateRequired = false)
        {
            using (Stream fs = File.OpenRead(filePath))
            {
                item.AttachFile(fs, leafName, overwrite, updateRequired);
            }
        }

        public static void AttachFile(this SPListItem item, Stream fileStream, string originalleafName, bool overwrite = false, bool updateRequired = false)
        {
            SPAttachmentCollection attachments = item.Attachments;
            SPList list = item.ParentList;
            SPWeb web = list.ParentWeb;

            if (!list.EnableAttachments)
            {
                //throw new SPException(string.Format("The specified file can not be added to the list \"{0}\" with disabled attachments.", list.Title));
                return;
            }

            string leafName = SPHelper.RemoveIllegalUrlCharacters(originalleafName);

            if (fileStream == null || fileStream.Length == 0)
            {
                throw new SPException("The specified file should not be empty.");
            }

            var maximumFileSize = web.Site.WebApplication.MaximumFileSize * 1024 * 1024;

            if (fileStream.Length > maximumFileSize)
            {
                throw new SPException(string.Format("The specified file is larger than the maximum supported file size: {0}.", SPUtility.FormatSize(maximumFileSize)));
            }

            if (overwrite)
            {
                string attachmentUrl = SPUrlUtility.CombineUrl(attachments.UrlPrefix, leafName);

                SPFile existingFile = web.GetFile(attachmentUrl);

                if (existingFile.Exists)
                {
                    existingFile.SaveBinary(fileStream);
                    return;
                }
            }

            BinaryReader reader = new BinaryReader(fileStream);

            if (updateRequired)
            {
                attachments.Add(leafName, reader.ReadBytes((int)fileStream.Length));
            }
            else
            {
                attachments.AddNow(leafName, reader.ReadBytes((int)fileStream.Length));
            }

            reader.Close();
        }

        public static IEnumerable<SPFile> GetAttachments(this SPListItem item)
        {
            SPAttachmentCollection attachments = item.Attachments;

            if (attachments == null || attachments.Count == 0)
            {
                return null;
            }

            SPList list = item.ParentList;
            SPWeb web = list.ParentWeb;

            SPFolder attachmentsFolder = web.GetFolder(attachments.UrlPrefix);
            return attachmentsFolder.Exists ? GetAttachments(attachmentsFolder, attachments) : null;
        }

        private static IEnumerable<SPFile> GetAttachments(SPFolder attachmentsFolder, SPAttachmentCollection listItemAttachments)
        {
            return from string file in listItemAttachments select attachmentsFolder.Files[file];
        }

        public static IEnumerable<string> GetAttachmentUrls(this SPListItem item)
        {
            if (item.Attachments != null)
            {
                return from string fileName in item.Attachments
                       orderby fileName
                       select SPUrlUtility.CombineUrl(item.Attachments.UrlPrefix, fileName);
            }

            return null;
        }

        public static string GetAttachmentsSize(this SPListItem item)
        {
            var attachments = GetAttachments(item);

            long size = 0;

            if (attachments != null)
            {
                size += attachments.Sum(attachment => attachment.Length);
            }

            return SPUtility.FormatSize(size);
        }

        public static void DeleteAttachments(this SPListItem item, bool recycling = false, bool updateRequired = false)
        {
            if (item.Attachments == null || item.Attachments.Count == 0) { return; }

            foreach (string attachment in item.Attachments.OfType<string>().ToList())
            {
                lock (item.Attachments.SyncRoot)
                {
                    if (recycling)
                    {
                        item.Attachments.Recycle(attachment);
                    }
                    else
                    {
                        item.Attachments.Delete(attachment);
                    }
                }
            }

            if (updateRequired)
            {
                item.Update();
            }
        }

        public static void SetExternalFieldValue(this SPListItem item, string fieldInternalName, string newValue)
        {
            if (item.Fields[fieldInternalName].TypeAsString == "BusinessData")
            {
                SPField field = item.Fields[fieldInternalName];
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(field.SchemaXml);
                //Get teh internal name of the SPBusinessDataField's identity column.
                String entityName = xmlData.FirstChild.Attributes["RelatedFieldWssStaticName"].Value;
                //Set the value of the identity column.
                item[entityName] = EntityInstanceIdEncoder.EncodeEntityInstanceId(new object[] { newValue });
                item[fieldInternalName] = newValue;
            }
            else
            {
                throw new InvalidOperationException(fieldInternalName + " is not of type BusinessData");
            }
        }

        public static string GetUrl(this SPListItem item, bool fullUrl = false)
        {
            return item.ParentList.GetDisplayFormUrl(Convert.ToUInt32(item.ID), fullUrl);
        }

        public static void SetContentType(this SPListItem item, SPContentTypeId contentTypeId)
        {
            SPContentType ct = item.ParentList.GetContentTypeById(contentTypeId);

            if (ct != null)
            {
                item[SPBuiltInFieldId.ContentTypeId] = ct.Id;
            }
        }

        public static void SetContentType(this SPListItem item, string contentTypeName)
        {
            SPContentType ct = item.ParentList.ContentTypes[contentTypeName];

            if (ct != null)
            {
                item[SPBuiltInFieldId.ContentTypeId] = ct.Id;
            }
        }

        /// <summary>
        /// Checks if the list contains a field with the specified name
        /// </summary>
        /// <param name="item">The SPListItem item</param>
        /// <param name="fieldName">The name of the Field</param>
        /// <returns><c>True</c> if the field exits, otherwisr <c>False</c></returns>
        public static bool Contains(this SPListItem item, string fieldName)
        {
            return item.Fields.ContainsField(fieldName);
        }

        #endregion
    }
}