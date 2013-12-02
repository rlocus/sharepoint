using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using SPCore.Helper;

namespace SPCore
{
    public static class SPContentTypeExtensions
    {
        public static SPFieldLink AddFieldLink(this SPContentType contentType, string fieldName)
        {
            SPField field = contentType.ParentWeb.AvailableFields.GetField(fieldName);

            SPFieldLink fieldLink = new SPFieldLink(field);

            if (contentType.FieldLinks[fieldLink.Id] == null)
            {
                contentType.FieldLinks.Add(fieldLink);
            }
            else
            {
                fieldLink = contentType.FieldLinks[fieldLink.Id];
            }

            return fieldLink;
        }

        public static SPFieldLink AddFieldLink(this SPContentType contentType, Guid fieldId)
        {
            SPField field = contentType.ParentWeb.AvailableFields[fieldId];
            SPFieldLink fieldLink = new SPFieldLink(field);

            if (contentType.FieldLinks[fieldLink.Id] == null)
            {
                contentType.FieldLinks.Add(fieldLink);
            }
            else
            {
                fieldLink = contentType.FieldLinks[fieldLink.Id];
            }

            return fieldLink;
        }

        public static void ClearFieldLinks(this SPContentType contentType)
        {
            foreach (SPFieldLink fl in contentType.FieldLinks.Cast<SPFieldLink>().ToList())
            {
                contentType.FieldLinks.Delete(fl.Id);
            }
        }

        public static SPWorkflowAssociation CreateWebWorkflowAssociation(this SPContentType contentType,
           SPWorkflowTemplate workflowTemplate, string assocName,
           string workflowTasksList, string workflowHistoryList,
           bool allowManual, bool autoStartCreate, bool autoStartChange, bool overwrite = false)
        {
            SPWorkflowAssociation workflowAssociation =
                  contentType.CreateWebWorkflowAssociation(workflowTemplate, assocName, workflowTasksList, workflowHistoryList,
                                                 overwrite,
                                                 assoc =>
                                                 {
                                                     assoc.AllowManual = allowManual;
                                                     assoc.AutoStartCreate = autoStartCreate;
                                                     assoc.AutoStartChange = autoStartChange;
                                                 });
            return workflowAssociation;
        }

        public static SPWorkflowAssociation CreateListWorkflowAssociation(this SPContentType contentType,
          SPWorkflowTemplate workflowTemplate, string assocName,
          SPList workflowTasksList, SPList workflowHistoryList,
          bool allowManual, bool autoStartCreate, bool autoStartChange, bool overwrite = false)
        {
            SPWorkflowAssociation workflowAssociation =
                 contentType.CreateListWorkflowAssociation(workflowTemplate, assocName, workflowTasksList, workflowHistoryList,
                                                overwrite,
                                                assoc =>
                                                {
                                                    assoc.AllowManual = allowManual;
                                                    assoc.AutoStartCreate = autoStartCreate;
                                                    assoc.AutoStartChange = autoStartChange;
                                                });
            return workflowAssociation;
        }

        public static SPWorkflowAssociation CreateListWorkflowAssociation(this SPContentType contentType,
         SPWorkflowTemplate workflowTemplate, string assocName,
         SPList workflowTasksList, SPList workflowHistoryList,
         bool overwrite = false, Action<SPWorkflowAssociation> action = null)
        {
            if (workflowTemplate == null) throw new ArgumentNullException("workflowTemplate");

            SPWorkflowAssociation workflowAssociation = null;
            // create the association
            SPWorkflowAssociation assoc =
                SPWorkflowAssociation.CreateListContentTypeAssociation(
                    workflowTemplate, assocName,
                    workflowTasksList, workflowHistoryList);

            if (action != null)
            {
                action(assoc);
            }

            //apply the association to the list
            if (contentType != null)
            {
                if (overwrite)
                {
                    SPWorkflowAssociation dupWfAssoc =
                        contentType.WorkflowAssociations.GetAssociationByName(assocName, contentType.ParentWeb.Locale);

                    if (dupWfAssoc != null)
                    {
                        if (dupWfAssoc.BaseId == workflowTemplate.Id)
                        {
                            contentType.WorkflowAssociations.Remove(dupWfAssoc);
                        }
                        else
                        {
                            throw new SPException(string.Format("Duplicate workflow name \"{0}\" is detected.", assocName));
                        }
                    }
                }

                workflowAssociation = contentType.WorkflowAssociations.Add(assoc);
            }

            return workflowAssociation;
        }

        public static SPWorkflowAssociation CreateWebWorkflowAssociation(this SPContentType contentType,
        SPWorkflowTemplate workflowTemplate, string assocName,
        string workflowTasksList, string workflowHistoryList,
        bool overwrite = false, Action<SPWorkflowAssociation> action = null)
        {
            if (workflowTemplate == null) throw new ArgumentNullException("workflowTemplate");

            SPWorkflowAssociation workflowAssociation = null;
            // create the association
            SPWorkflowAssociation assoc =
                SPWorkflowAssociation.CreateWebContentTypeAssociation(
                    workflowTemplate, assocName,
                    workflowTasksList, workflowHistoryList);

            if (action != null)
            {
                action(assoc);
            }

            //apply the association to the list
            if (contentType != null)
            {
                if (overwrite)
                {
                    SPWorkflowAssociation dupWfAssoc =
                        contentType.WorkflowAssociations.GetAssociationByName(assocName, contentType.ParentWeb.Locale);

                    if (dupWfAssoc != null)
                    {
                        if (dupWfAssoc.BaseId == workflowTemplate.Id)
                        {
                            contentType.WorkflowAssociations.Remove(dupWfAssoc);
                        }
                        else
                        {
                            throw new SPException(string.Format("Duplicate workflow name \"{0}\" is detected.", assocName));
                        }
                    }
                }

                workflowAssociation = contentType.WorkflowAssociations.Add(assoc);
            }

            return workflowAssociation;
        }

        public static void Rename(this SPContentType contentType, string replacementName)
        {
            contentType.UpdateWithAction(ct => ct.Name = replacementName, true);
        }

        public static void UpdateWithAction(this SPContentType contentType, Action<SPContentType> action, bool updateChildren = false, bool throwOnSealedOrReadOnly = false)
        {
            if (action == null)
            {
                return;
            }

            if (contentType.ParentList == null)
            {
                updateChildren = false;
            }

            action(contentType);
            contentType.Update(updateChildren, throwOnSealedOrReadOnly);

            if (contentType.ParentList != null) return;

            Dictionary<string, List<string>> listUrls = SPHelper.GetListUrls(contentType);

            foreach (string webUrl in listUrls.Keys)
            {
                using (SPWeb web = contentType.ParentWeb.Site.OpenWeb(webUrl))
                {
                    using (new Unsafe(web))
                    {
                        foreach (string listUrl in listUrls[webUrl])
                        {
                            SPList list = web.GetListByUrl(listUrl);

                            SPContentType ct = list.GetContentTypeById(contentType.Id);

                            if (ct != null)
                            {
                                action(ct);
                                ct.Update(throwOnSealedOrReadOnly);
                            }
                        }
                    }
                }
            }
        }

        public static bool IsWithinList(this SPContentType contentType)
        {
            return (contentType.ParentList != null);
        }
    }
}
