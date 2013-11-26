using System;
using System.Collections.Specialized;
using Microsoft.SharePoint;

namespace SPCore.Extensions
{
    public static class SPUserExtensions
    {
        public static SPListItem GetUserItem(this SPUser user)
        {
            return null == user ? null : user.ParentWeb.SiteUserInfoList.GetItemById(user.ID);
        }

        public static StringDictionary GetUserInfo(this SPUser user)
        {
            StringDictionary dic = new StringDictionary
                                       {
                                           {"Title", null},
                                           {"Name", null},
                                           {"FirstName", null},
                                           {"LastName", null},
                                           {"Picture", null},
                                           {"Email", null},
                                           {"Department", null},
                                           {"Job", null},
                                           {"WorkPhone", null},
                                           {"MobilePhone", null},
                                           {"WebSite", null},
                                           {"Office", null},
                                           {"Notes", null},
                                           {"IsSiteAdmin", null}
                                       };

            SPListItem item = GetUserItem(user);

            if (item != null)
            {
                dic["Title"] = Convert.ToString(item.TryGetValue(SPBuiltInFieldId.Title));
                dic["Name"] = Convert.ToString(item.TryGetValue(SPBuiltInFieldId.Name));
                dic["FirstName"] = Convert.ToString(item.TryGetValue("FirstName"));
                dic["LastName"] = Convert.ToString(item.TryGetValue("LastName"));
                dic["Picture"] = Convert.ToString(item.TryGetValue("Picture"));
                dic["Email"] = Convert.ToString(item.TryGetValue(SPBuiltInFieldId.EMail));
                dic["Department"] = Convert.ToString(item.TryGetValue(SPBuiltInFieldId.Department));
                dic["Job"] = Convert.ToString(item.TryGetValue(SPBuiltInFieldId.JobTitle));
                dic["WorkPhone"] = Convert.ToString(item.TryGetValue("WorkPhone"));
                dic["MobilePhone"] = Convert.ToString(item.TryGetValue(SPBuiltInFieldId.MobilePhone));
                dic["WebSite"] = Convert.ToString(item.TryGetValue("WebSite"));
                dic["Office"] = Convert.ToString(item.TryGetValue("Office"));
                dic["Notes"] = Convert.ToString(item.TryGetValue(SPBuiltInFieldId.Notes));
                dic["IsSiteAdmin"] = Convert.ToString(item.TryGetValue(SPBuiltInFieldId.IsSiteAdmin));
            }

            return dic;
        }
    }
}
