﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PlatoCore.Security.Abstractions
{
    public static class AuthorizationServiceExtensions
    {

        public static Task<bool> AuthorizeAsync(
            this IAuthorizationService service,
            ClaimsPrincipal principal,
            IPermission permission)
            => AuthorizeAsync(service, principal, null, permission);
        
        public static async Task<bool> AuthorizeAsync(
            this IAuthorizationService service,
            ClaimsPrincipal principal,
            object resource,
            IPermission permission)
        {
            var result = await service.AuthorizeAsync(
                principal, 
                resource, 
                new PermissionRequirement(permission));
            return result.Succeeded;
        }

    }

}
