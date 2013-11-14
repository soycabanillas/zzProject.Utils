﻿// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace zzProject.Utils.Linq.OData
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Defines a set of methods that can participate in query deserialization.
    /// </summary>
    internal abstract class QueryResolver
    {
        /// <summary>
        /// Called to attempt to resolve unresolved member references during query deserialization.
        /// </summary>
        /// <param name="type">The Type the member is expected on.</param>
        /// <param name="member">The member name.</param>
        /// <param name="instance">The instance to form the MemberExpression on.</param>
        /// <returns>A MemberExpression if the member can be resolved, null otherwise.</returns>
        public abstract MemberExpression ResolveMember(Type type, string member, Expression instance);
    }
}
