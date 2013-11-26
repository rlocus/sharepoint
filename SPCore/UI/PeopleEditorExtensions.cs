using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SPCore.UI
{
    public static class PeopleEditorExtensions
    {
        private static bool IsEntityUser(PickerEntity entity)
        {
            return entity.EntityData != null && entity.EntityData.Count > 0 &&
                   entity.EntityData["PrincipalType"].ToString() == "User";
        }

        public static IEnumerable<SPUser> GetUsers(this PeopleEditor peopleEditor, SPWeb web)
        {
            foreach (PickerEntity pickerEntity in peopleEditor.ResolvedEntities)
            {
                if (IsEntityUser(pickerEntity))
                {
                    SPUser user = web.EnsureUser(pickerEntity.Description);
                    yield return user;
                }
            }
        }

        public static IEnumerable<SPGroup> GetGroups(this PeopleEditor peopleEditor, SPWeb web)
        {
            foreach (PickerEntity pickerEntity in peopleEditor.ResolvedEntities)
            {
                if (!IsEntityUser(pickerEntity))
                {
                    SPGroup group = web.SiteGroups.GetGroup(pickerEntity.Description);

                    if (group != null)
                    {
                        yield return group;
                    }
                }
            }
        }

        public static IEnumerable<SPPrincipal> GetUsersOrGroups(this PeopleEditor peopleEditor, SPWeb web)
        {
            foreach (PickerEntity pickerEntity in peopleEditor.ResolvedEntities)
            {
                if (IsEntityUser(pickerEntity))
                {
                    SPUser user = web.EnsureUser(pickerEntity.Description);
                    yield return user;
                }
                else
                {
                    SPGroup group = web.SiteGroups.GetGroup(pickerEntity.Description);

                    if (group != null)
                    {
                        yield return group;
                    }
                }
            }
        }

        public static void SetEntities(this PeopleEditor peopleEditor, IEnumerable<string> entities)
        {
            ArrayList entityArrayList = new ArrayList();

            if (entities != null)
                foreach (string entitiy in entities)
                {
                    PickerEntity entity = peopleEditor.ValidateEntity(new PickerEntity { Key = entitiy });
                    entityArrayList.Add(entity);
                }

            peopleEditor.UpdateEntities(entityArrayList);
        }
    }
}
