﻿// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

#if NET35 || NET40
namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class TypeExtensions
    {

        public static object GetValue(this PropertyInfo info, object obj)
        {
            return info.GetValue(obj, null);
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider info)
            where T : class
        {
            var attrs = info.GetCustomAttributes(typeof(T), inherit: false);
            if (attrs.Length == 0)
                return null;
            return attrs[0] as T;
        }
    }
}
#endif