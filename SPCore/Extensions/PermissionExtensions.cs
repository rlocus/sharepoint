using System;
using System.Linq;
using Microsoft.SharePoint;

namespace SPCore
{
    public static class PermissionExtensions
    {
        public static SPGroup GetGroup(this SPGroupCollection groups, string groupName)
        {
            return groups.Cast<SPGroup>().FirstOrDefault(
                @group =>
                string.Equals(group.Name, groupName, StringComparison.OrdinalIgnoreCase));
        }

        public static bool GroupExists(this SPGroupCollection groups, string groupName)
        {
            return GetGroup(groups, groupName) != null;
        }

        public static bool BelongsToGroup(this SPUser user, string groupName)
        {
            return user.Groups.Cast<SPGroup>().Any(@g => g.Name == groupName);
        }

        public static bool AssignmentExists(this SPSecurableObject securableObject, SPPrincipal principal)
        {
            return (GetAssignment(securableObject, principal) != null);
        }

        public static SPRoleAssignment GetAssignment(this SPSecurableObject securableObject, SPPrincipal principal)
        {
            var roleAssignments = securableObject.RoleAssignments;

            if (securableObject.RoleAssignments == null)
                return null;
            try
            {
                return roleAssignments.GetAssignmentByPrincipal(principal);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static void AssignPermissions(this SPSecurableObject securableObject, SPPrincipal principal, SPRoleType roleType, bool replaceAllDefinitions = false, bool copyRoleAssignments = false, bool clearSubscopes = true)
        {
            SPWeb web = principal.ParentWeb;
            SPRoleDefinition roleDefinition = web.RoleDefinitions.GetByType(roleType);
            AssignPermissions(securableObject, principal, roleDefinition, replaceAllDefinitions, copyRoleAssignments, clearSubscopes);
        }

        public static void AssignPermissions(this SPSecurableObject securableObject, SPPrincipal principal, string roleDefinitionName, bool replaceAllDefinitions = false, bool copyRoleAssignments = false, bool clearSubscopes = true)
        {
            SPWeb web = principal.ParentWeb;
            SPRoleDefinition roleDefinition = web.RoleDefinitions[roleDefinitionName];
            AssignPermissions(securableObject, principal, roleDefinition, replaceAllDefinitions, copyRoleAssignments, clearSubscopes);
        }

        public static void AssignPermissions(this SPSecurableObject securableObject, SPPrincipal principal, SPRoleDefinition roleDefinition, bool replaceAllDefinitions = false, bool copyRoleAssignments = false, bool clearSubscopes = true)
        {
            if (securableObject is SPWeb)
            {
                if (!securableObject.HasUniqueRoleAssignments)
                {
                    securableObject.BreakRoleInheritance(copyRoleAssignments, clearSubscopes);
                }
            }
            else
            {
                securableObject.BreakRoleInheritance(copyRoleAssignments, clearSubscopes);
            }

            SPRoleAssignment roleAssignment = GetAssignment(securableObject, principal);

            if (roleAssignment == null)
            {
                roleAssignment = new SPRoleAssignment(principal);
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
            }

            if (replaceAllDefinitions)
            {
                try
                {
                    roleAssignment.RoleDefinitionBindings.RemoveAll();
                    roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                    roleAssignment.Update();
                }
                catch (ArgumentException)
                {
                    //Note: fix for 'Cannot update a permission level assignment that is not part of a permission level assignment collection.'
                    securableObject.RoleAssignments.Add(roleAssignment);
                }
            }
            else
            {
                securableObject.RoleAssignments.Add(roleAssignment);
            }
        }

        public static void ClearRoleAssignments(this SPSecurableObject securableObject)
        {
            if (!securableObject.HasUniqueRoleAssignments) return;

            for (int i = securableObject.RoleAssignments.Count - 1; i >= 0; i--)
            {
                securableObject.RoleAssignments.Remove(i);
            }
        }
    }
}
