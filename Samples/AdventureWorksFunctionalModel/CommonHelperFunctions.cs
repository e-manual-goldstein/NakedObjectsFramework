using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksModel
{
    public static class CommonHelperFunctions
    {

        public static T SetRowGuid<T>(this T obj, Guid newGuid) where T: IHasRowGuid
        {
            return obj.With(x => x.rowguid, newGuid);
        }

        public static T UpdateModifiedDate<T>(this T obj, DateTime  now) where T : IHasModifiedDate
        {
            return obj.With(x => x.ModifiedDate, now);
        }

        //Guards against the object being null
        public static string CreateTitle<T>(this T obj, string exp) where T : class
        {
            return obj is null ? "" : exp;
        }
    }
}
