﻿using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Roles
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageRoles =
            new Permission("ManageRoles", "Can manage roles");
        
        public static readonly Permission AddRoles = 
            new Permission("AddRoles", "Can add new roles");

        public static readonly Permission EditRoles =
            new Permission("EditRoles", "Can edit existing roles");

        public static readonly Permission DeleteRoles =
            new Permission("DeleteRoles", "Can delete existing roles");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageRoles,
                AddRoles,
                EditRoles,
                DeleteRoles,
                StandardPermissions.AdminAccess
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        ManageRoles,
                        AddRoles,
                        EditRoles,
                        DeleteRoles,
                        StandardPermissions.AdminAccess
                    }
                }
            };

        }

    }

}
