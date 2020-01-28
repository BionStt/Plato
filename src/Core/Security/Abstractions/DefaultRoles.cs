﻿namespace PlatoCore.Security.Abstractions
{
    public static class DefaultRoles
    {

        public const string Anonymous = "Anonymous";
        public const string Member = "Member";
        public const string Staff = "Staff";
        public const string Administrator = "Administrator";
        
        public static string[] ToList() => new[]
        {
            Anonymous,
            Member,
            Staff,
            Administrator
        };

    }
}
