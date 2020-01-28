﻿using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Discuss.Categories.Moderators
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewModerationQueue =
            new Permission("ViewModerationQueue", "View moderation queue");

        public static readonly Permission ViewSpamQueue =
            new Permission("ViewSpamQueue", "View SPAM queue");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewModerationQueue,
                ViewSpamQueue
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
                        ViewModerationQueue,
                        ViewSpamQueue
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ViewModerationQueue,
                        ViewSpamQueue
                    }
                }
            };

        }

    }

}
