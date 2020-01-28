﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PlatoCore.Abstractions.Extensions
{
    public static class TypeExtensions
    {

        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }

        public static bool IsAnonymousType(this Type t)
        {
            return t != null && t.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();
        }
    }
}
