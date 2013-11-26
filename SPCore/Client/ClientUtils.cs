using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Workflow;
using Microsoft.SharePoint.Utilities;
using File = Microsoft.SharePoint.Client.File;

namespace SPCore.Client
{
    public static class ClientUtils
    {
        public static File CreatePage(ClientContext context, string pageName, byte[] content, bool overwrite)
        {
            Web site = context.Web;
            List pages = site.Lists.EnsureSitePagesLibrary();

            return CreatePage(pages, pageName, content, overwrite);
        }

        public static File CreatePage(List pages, string pageName, byte[] content, bool overwrite)
        {
            FileCreationInformation fileCreateInfo = new FileCreationInformation
                                                         {
                                                             Overwrite = overwrite,
                                                             Url = string.Format("{0}.aspx", pageName),
                                                             Content = content ?? Encoding.ASCII.GetBytes("")
                                                         };
            return CreatePage(pages, fileCreateInfo);
        }

        public static File CreatePage(List pages, FileCreationInformation fileCreateInfo)
        {
            File page = pages.RootFolder.Files.Add(fileCreateInfo);

            pages.Context.Load(page);
            pages.Context.ExecuteQuery();

            return page;
        }

        public static List CreateList(this Web site, string title, string url, string desc, ListTemplateType template, bool onQuickLaunch)
        {
            ListCreationInformation listCreateInfo = new ListCreationInformation
                                                         {
                                                             Title = title,
                                                             TemplateType = (int)template,
                                                             Description = desc,
                                                             Url = url
                                                         };

            if (onQuickLaunch)
            {
                listCreateInfo.QuickLaunchOption = QuickLaunchOptions.On;
            }

            return CreateList(site, listCreateInfo);
        }

        public static List CreateList(this Web site, ListCreationInformation listCreateInfo)
        {
            List list = site.Lists.Add(listCreateInfo);

            site.Context.Load(list);
            site.Context.ExecuteQuery();

            return list;
        }

        public static void DeleteList(this Web site, string listName)
        {
            List list = site.Lists.GetByTitle(listName);
            list.DeleteObject();
            site.Context.ExecuteQuery();
        }

        public static void DeleteList(this Web site, Guid listId)
        {
            List list = site.Lists.GetById(listId);
            list.DeleteObject();
            site.Context.ExecuteQuery();
        }

        public static Group CreateSiteGroup(this Web site, string groupName, string groupDesc)
        {
            GroupCreationInformation groupCreateInfo = new GroupCreationInformation
                                                           {
                                                               Title = groupName,
                                                               Description = groupDesc
                                                           };
            return CreateSiteGroup(site, groupCreateInfo);
        }

        public static Group CreateSiteGroup(this Web site, GroupCreationInformation groupCreateInfo)
        {
            Group group = site.SiteGroups.Add(groupCreateInfo);

            site.Context.Load(group);
            site.Context.ExecuteQuery();

            return group;
        }

        public static void DeleteSiteGroup(this Web site, int groupId)
        {
            Group group = site.SiteGroups.GetById(groupId);
            site.SiteGroups.Remove(group);
            site.Context.ExecuteQuery();
        }

        public static void DeleteSiteGroup(this Web site, string groupName)
        {
            Group group = site.GetSiteGroup(groupName);

            if (group != null)
            {
                site.SiteGroups.Remove(group);
                site.Context.ExecuteQuery();
            }
        }

        public static Group GetSiteGroup(this Web site, string groupName)
        {
            var query = site.Context.LoadQuery(site.SiteGroups.Where(@group => group.Title == groupName));
            site.Context.ExecuteQuery();
            return query.FirstOrDefault();
        }

        public static TF CreateField<TF>(
           this FieldCollection fields,
           FieldType fieldType,
           Guid fieldId,
           string name,
           string displayName,
           bool required,
           Action<TF> action = null)
           where TF : Field
        {
            string fieldXml =
                   string.Format(
                       @"<Field ID=""{0}"" Name=""{1}"" StaticName=""{1}"" DisplayName=""{2}"" Type=""{3}"" Overwrite=""TRUE"" SourceID=""http://schemas.microsoft.com/sharepoint/v3"" />",
                   fieldId, name, displayName, fieldType);

            Field newField = fields.AddFieldAsXml(fieldXml, true, AddFieldOptions.DefaultValue);

            TF field = fields.Context.CastTo<TF>(newField);
            field.Required = required;

            if (action != null)
            {
                action(field);
            }

            field.Update();

            fields.Context.ExecuteQuery();

            return field;
        }

        public static void DeleteField(this FieldCollection fields, string name)
        {
            Field field = fields.GetByInternalNameOrTitle(name);
            field.DeleteObject();
            fields.Context.ExecuteQuery();
        }

        public static void DeleteField(this FieldCollection fields, Guid fieldId)
        {
            Field field = fields.GetById(fieldId);
            field.DeleteObject();
            fields.Context.ExecuteQuery();
        }

        public static File CreateFile(this Folder folder, string fileSourcePath, bool overwrite)
        {
            byte[] byteFile = System.IO.File.ReadAllBytes(fileSourcePath);
            string fileName = System.IO.Path.GetFileName(fileSourcePath);

            return folder.CreateFile(fileName, byteFile, overwrite);
        }

        public static File CreateFile(this Folder folder, string fileName, byte[] content, bool overwrite)
        {
            if (!folder.IsPropertyAvailable("ServerRelativeUrl"))
            {
                folder.Context.Load(folder);
                folder.Context.ExecuteQuery();
            }

            FileCreationInformation fileCreateInfo = new FileCreationInformation
                                                         {
                                                             Content = content,
                                                             Overwrite = overwrite,
                                                             Url = SPUrlUtility.CombineUrl(folder.ServerRelativeUrl, fileName)
                                                         };
            return folder.CreateFile(fileCreateInfo);
        }

        public static File CreateFile(this Folder folder, FileCreationInformation fileCreateInfo)
        {
            File file = folder.Files.Add(fileCreateInfo);

            folder.Context.Load(file);
            folder.Context.ExecuteQuery();

            return file;
        }

        public static void DeleteFile(this Web site, string fileServerRelativeUrl)
        {
            File file = site.GetFileByServerRelativeUrl(fileServerRelativeUrl);

            file.DeleteObject();
            site.Context.ExecuteQuery();
        }

        public static RoleDefinition CreateRoleDefinition(this RoleDefinitionCollection roleDefinitions, BasePermissions permissions, string name, string description, int order)
        {
            RoleDefinitionCreationInformation roleDefCreateInfo = new RoleDefinitionCreationInformation
                                                                      {
                                                                          BasePermissions = permissions,
                                                                          Name = name,
                                                                          Description = description,
                                                                          Order = order
                                                                      };

            return roleDefinitions.CreateRoleDefinition(roleDefCreateInfo);
        }

        public static RoleDefinition CreateRoleDefinition(this RoleDefinitionCollection roleDefinitions, RoleDefinitionCreationInformation roleDefCreateInfo)
        {
            RoleDefinition roleDefinition = roleDefinitions.Add(roleDefCreateInfo);

            roleDefinitions.Context.Load(roleDefinition);
            roleDefinitions.Context.ExecuteQuery();

            return roleDefinition;
        }

        public static void DeleteRoleDefinition(this RoleDefinitionCollection roleDefinitions, string name)
        {
            RoleDefinition roleDefinition = roleDefinitions.GetByName(name);
            roleDefinition.DeleteObject();
            roleDefinitions.Context.ExecuteQuery();
        }

        public static void DeleteRoleDefinition(this RoleDefinitionCollection roleDefinitions, int id)
        {
            RoleDefinition roleDefinition = roleDefinitions.GetById(id);
            roleDefinition.DeleteObject();
            roleDefinitions.Context.ExecuteQuery();
        }

        public static ContentType CreateContentType(this Web site, string parentContentTypeId, string name, string groupName, string desc)
        {
            ContentType parentContentType = site.ContentTypes.GetById(parentContentTypeId);

            return CreateContentType(site, parentContentType, name, groupName, desc);
        }

        public static ContentType CreateContentType(this Web site, ContentType parentContentType, string name, string groupName, string desc)
        {
            ContentTypeCreationInformation contentTypeCreateInfo = new ContentTypeCreationInformation
                                                                       {
                                                                           Name = name,
                                                                           ParentContentType = parentContentType,
                                                                           Group = groupName,
                                                                           Description = desc
                                                                       };
            return CreateContentType(site, contentTypeCreateInfo);
        }

        public static ContentType CreateContentType(this Web site, ContentTypeCreationInformation contentTypeCreateInfo)
        {
            ContentTypeCollection contentTypes = site.ContentTypes;
            ContentType contentType = contentTypes.Add(contentTypeCreateInfo);

            site.Context.Load(contentType);
            site.Context.ExecuteQuery();

            return contentType;
        }

        public static void DeleteContentType(this Web site, string name)
        {
            ContentType contentType = GetContentType(site, name);

            if (contentType != null)
            {
                contentType.DeleteObject();
                site.Context.ExecuteQuery();
            }
        }

        public static ContentType GetContentType(this Web site, string name)
        {
            var query = site.Context.LoadQuery(site.AvailableContentTypes.Where(ct => ct.Name == name));

            site.Context.ExecuteQuery();

            ContentType contentType = query.FirstOrDefault();

            return contentType;
        }

        public static View CreateView(this List list, string title, string[] viewFields, ViewType viewTypeKind,
            string query, uint rowLimit, bool paged, bool personalView, bool isDefault)
        {
            ViewCreationInformation viewCreationInformation = new ViewCreationInformation
                                                                  {
                                                                      Title = title,
                                                                      ViewFields = viewFields,
                                                                      ViewTypeKind = viewTypeKind,
                                                                      Query = query,
                                                                      RowLimit = rowLimit,
                                                                      Paged = paged,
                                                                      PersonalView = personalView,
                                                                      SetAsDefaultView = isDefault
                                                                  };
            return CreateView(list, viewCreationInformation);
        }


        public static View CreateView(this List list, ViewCreationInformation viewCreationInformation)
        {
            View view = list.Views.Add(viewCreationInformation);

            list.Context.Load(view);
            list.Context.ExecuteQuery();

            return view;
        }

        public static void DeleteView(this List list, string viewName)
        {
            View view = list.Views.GetByTitle(viewName);

            view.DeleteObject();
            list.Context.ExecuteQuery();
        }

        public static void DeleteView(this List list, Guid viewId)
        {
            View view = list.Views.GetById(viewId);

            view.DeleteObject();
            list.Context.ExecuteQuery();
        }

        public static ListItem CreateItem(this List list, ListItemCreationInformation itemCreateInfo, Dictionary<string, object> entries)
        {
            ListItem listItem = list.AddItem(itemCreateInfo);

            foreach (KeyValuePair<string, object> entry in entries)
            {
                listItem[entry.Key] = entry.Value;
            }

            listItem.Update();

            list.Context.ExecuteQuery();

            return listItem;
        }

        public static void UpdateItem(this List list, int itemId, Dictionary<string, object> entries)
        {
            ListItem listItem = list.GetItemById(itemId);

            foreach (KeyValuePair<string, object> entry in entries)
            {
                listItem[entry.Key] = entry.Value;
            }

            listItem.Update();

            list.Context.ExecuteQuery();
        }

        public static void DeleteItem(this List list, int itemId)
        {
            ListItem listItem = list.GetItemById(itemId);

            listItem.DeleteObject();

            list.Context.ExecuteQuery();
        }

        public static WorkflowAssociation CreateListWorkflowAssociation(this Web site, string listName, Guid workflowTemplateId, string assocName,
            string workflowTasksListName, string workflowHistoryListName, bool allowManual, bool autoStartCreate, bool autoStartChange)
        {
            WorkflowTemplate wfTemplate = site.WorkflowTemplates.GetById(workflowTemplateId);
            site.Context.Load(wfTemplate);

            return site.CreateListWorkflowAssociation(listName, wfTemplate, assocName, workflowTasksListName,
                                                      workflowHistoryListName, allowManual, autoStartCreate,
                                                      autoStartChange);
        }

        public static WorkflowAssociation CreateListWorkflowAssociation(this Web site, string listName, string workflowTemplateName, string assocName,
        string workflowTasksListName, string workflowHistoryListName,
        bool allowManual, bool autoStartCreate, bool autoStartChange)
        {
            WorkflowTemplate wfTemplate = site.WorkflowTemplates.GetByName(workflowTemplateName);
            site.Context.Load(wfTemplate);

            return site.CreateListWorkflowAssociation(listName, wfTemplate, assocName, workflowTasksListName,
                                                      workflowHistoryListName, allowManual, autoStartCreate,
                                                      autoStartChange);
        }

        public static WorkflowAssociation CreateListWorkflowAssociation(this Web site, string listName, WorkflowTemplate workflowTemplate, string assocName,
         string workflowTasksListName, string workflowHistoryListName,
         bool allowManual, bool autoStartCreate, bool autoStartChange)
        {
            var wfc = new WorkflowAssociationCreationInformation
                          {
                              TaskList = site.Lists.GetByTitle(workflowTasksListName),
                              HistoryList = site.Lists.GetByTitle(workflowHistoryListName),
                              Name = assocName,
                              Template = workflowTemplate
                          };

            List list = site.Lists.GetByTitle(listName);

            return list.CreateListWorkflowAssociation(wfc, allowManual, autoStartCreate, autoStartChange);
        }

        public static WorkflowAssociation CreateListWorkflowAssociation(this List list, WorkflowAssociationCreationInformation workflowAssociationCreation,
            bool allowManual, bool autoStartCreate, bool autoStartChange)
        {
            WorkflowAssociation wf = list.WorkflowAssociations.Add(workflowAssociationCreation);
            wf.AllowManual = allowManual;
            wf.AutoStartChange = autoStartCreate;
            wf.AutoStartCreate = autoStartChange;
            wf.Enabled = true;
            //string assocData = GetAssociationXml(); // internal method
            // wf.AssociationData = assocData; // is never updated
            wf.Update();

            list.Context.Load(wf);
            list.Context.ExecuteQuery();

            return wf;
        }

        public static WorkflowAssociation GetListWorkflowAssociation(this List list, string assocName)
        {
            var wa = list.WorkflowAssociations.GetByName(assocName);

            list.Context.Load(wa);
            list.Context.ExecuteQuery();

            return wa;
        }

        public static WorkflowAssociation GetListWorkflowAssociation(this List list, Guid assocId)
        {
            var wa = list.WorkflowAssociations.GetById(assocId);

            list.Context.Load(wa);
            list.Context.ExecuteQuery();

            return wa;
        }
    }
}
