using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SPCore
{
    public static class SPListCollectionExtensions
    {
        public static SPList TryGetListByInternalName(this SPListCollection lists, string internalName)
        {
            return lists.GetListsByInternalName<SPList>(internalName).SingleOrDefault();
        }

        public static IEnumerable<TList> GetListsByInternalName<TList>(this SPListCollection lists, string internalName)
             where TList : SPList
        {
            return lists.Cast<TList>().Where(l => l.RootFolder.Name == internalName);
        }

        public static IEnumerable<TList> GetListsByInternalName<TList>(this SPListCollection lists, string internalName, SPBaseType baseType)
             where TList : SPList
        {
            return lists.Web.GetLists<TList>(baseType).Where(l => l.RootFolder.Name == internalName);
        }

        public static IEnumerable<TList> GetListsByInternalName<TList>(this SPListCollection lists, string internalName, SPBaseType baseType, SPListTemplateType templateType)
            where TList : SPList
        {
            return lists.GetListsByInternalName<TList>(internalName, baseType).Where(l => l.BaseTemplate == templateType);
        }

        public static IEnumerable<TList> GetListsByInternalName<TList>(this SPListCollection lists, string internalName, SPListTemplateType templateType)
            where TList : SPList
        {
            return lists.GetLists<TList>(templateType).Where(l => l.RootFolder.Name == internalName);
        }

        public static IEnumerable<TList> GetListsByInternalName<TList>(this SPListCollection lists, string internalName, int templateType)
            where TList : SPList
        {
            return lists.GetLists<TList>(templateType).Where(l => l.RootFolder.Name == internalName);
        }

        public static IEnumerable<TList> GetListsByInternalName<TList>(this SPListCollection lists, string internalName, SPListTemplate template)
           where TList : SPList
        {
            return lists.GetListsByInternalName<TList>(internalName, template.BaseType, template.Type);
        }

        public static IEnumerable<TList> GetLists<TList>(this SPListCollection lists, SPListTemplateType templateType)
           where TList : SPList
        {
            return lists.OfType<TList>().Where(lst => lst.BaseTemplate == templateType);
        }

        public static IEnumerable<TList> GetLists<TList>(this SPListCollection lists, int templateType)
           where TList : SPList
        {
            return lists.OfType<TList>().Where(lst => (int)lst.BaseTemplate == templateType);
        }
    }
}
