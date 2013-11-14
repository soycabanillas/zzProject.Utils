using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zzProject.Utils.Linq.OData
{
    public class Query
    {
        public static IQueryable Translate(IQueryable query, Uri uri, bool skipTopEnabled = true)
        {
            return ODataQueryDeserializer.Deserialize(query, uri, skipTopEnabled);
        }
    }
}
