// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace zzProject.Utils.Linq.OData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Used to deserialize a set of string based query operations into expressions and
    /// compose them over a specified query.
    /// </summary>
    internal static class ODataQueryDeserializer
    {
        /// <summary>
        /// Deserializes the query operations in the specified Uri and applies them
        /// to the specified IQueryable.
        /// </summary>
        /// <param name="query">The root query to compose the deserialized query over.</param>
        /// <param name="uri">The request Uri containing the query operations.</param>
        /// <returns>The resulting IQueryable with the deserialized query composed over it.</returns>
        public static IQueryable Deserialize(IQueryable query, Uri uri, bool skipTopEnabled = true)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            ServiceQuery serviceQuery = GetServiceQuery(uri);

            return Deserialize(query, serviceQuery.QueryParts, null);
        }

        /// <summary>
        /// Deserializes the query operations in the specified Uri and returns an IQueryable
        /// with a manufactured query root with those operations applied.
        /// </summary>
        /// <typeparam name="T">The element type of the query</typeparam>
        /// <param name="uri">The request Uri containing the query operations.</param>
        /// <returns>The resulting IQueryable with the deserialized query composed over it.</returns>
        public static IQueryable<T> Deserialize<T>(Uri uri, bool skipTopEnabled = true)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            return (IQueryable<T>)Deserialize(typeof(T), uri, skipTopEnabled);
        }

        /// <summary>
        /// Deserializes the query operations in the specified Uri and returns an IQueryable
        /// with a manufactured query root with those operations applied.
        /// </summary>
        /// <param name="elementType">The element type of the query</param>
        /// <param name="uri">The request Uri containing the query operations.</param>
        /// <returns>The resulting IQueryable with the deserialized query composed over it.</returns>
        public static IQueryable Deserialize(Type elementType, Uri uri, bool skipTopEnabled = true)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            ServiceQuery serviceQuery = GetServiceQuery(uri);

            Array array = Array.CreateInstance(elementType, 0);
            IQueryable baseQuery = ((IEnumerable)array).AsQueryable();

            return Deserialize(baseQuery, serviceQuery.QueryParts, null);
        }

        internal static IQueryable Deserialize(IQueryable query, IEnumerable<ServiceQueryPart> queryParts, bool skipTopEnabled = true)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (queryParts == null)
            {
                throw new ArgumentNullException("queryParts");
            }

            return Deserialize(query, queryParts, null, skipTopEnabled);
        }

        internal static IQueryable Deserialize(IQueryable query, IEnumerable<ServiceQueryPart> queryParts, QueryResolver queryResolver, bool skipTopEnabled = true)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (queryParts == null)
            {
                throw new ArgumentNullException("queryParts");
            }

            foreach (ServiceQueryPart part in queryParts)
            {
                switch (part.QueryOperator)
                {
                    case "filter":
                        query = DynamicQueryable.Where(query, part.Expression, queryResolver);
                        break;
                    case "orderby":
                        query = DynamicQueryable.OrderBy(query, part.Expression, queryResolver);
                        break;
                    case "skip":
                        if (skipTopEnabled)
                        {
                            query = DynamicQueryable.Skip(query, Convert.ToInt32(part.Expression, System.Globalization.CultureInfo.InvariantCulture));
                        }
                        break;
                    case "top":
                        if (skipTopEnabled)
                        {
                            query = DynamicQueryable.Take(query, Convert.ToInt32(part.Expression, System.Globalization.CultureInfo.InvariantCulture));
                        }
                        break;
                }
            }

            return query;
        }

        internal static ServiceQuery GetServiceQuery(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            NameValueCollection queryPartCollection = HttpUtility.ParseQueryString(uri.Query);

            List<ServiceQueryPart> serviceQueryParts = new List<ServiceQueryPart>();
            foreach (string queryPart in queryPartCollection)
            {
                if (queryPart == null || !queryPart.StartsWith("$", StringComparison.Ordinal))
                {
                    // not a special query string
                    continue;
                }

                foreach (string value in queryPartCollection.GetValues(queryPart))
                {
                    ServiceQueryPart serviceQueryPart = new ServiceQueryPart(queryPart.Substring(1), value);
                    serviceQueryParts.Add(serviceQueryPart);
                }
            }

            // Query parts for OData need to be ordered $filter, $orderby, $skip, $top. For this
            // set of query operators, they are already in alphabetical order, so it suffices to
            // order by operator name. In the future if we support other operators, this may need
            // to be reexamined.
            serviceQueryParts = serviceQueryParts.OrderBy(p => p.QueryOperator).ToList();

            ServiceQuery serviceQuery = new ServiceQuery()
            {
                QueryParts = serviceQueryParts,
            };

            return serviceQuery;
        }
    }
}
