namespace NakedFunctions
{
    public static class TitleHelpers
    {
        //Guards against the object being null
        public static string CreateTitle<T>(this T obj, string exp) where T : class
        {
            return (obj == null) ? "" : exp;
        }
    }
}
