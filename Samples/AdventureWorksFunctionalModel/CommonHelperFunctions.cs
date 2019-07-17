using System;
using NakedFunctions;

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
    }
}
