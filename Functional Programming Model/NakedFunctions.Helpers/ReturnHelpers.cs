namespace NakedFunctions
{

    /// <summary>
    /// Helper functions for returning items from action functions
    /// </summary>
    /// 
    public static class ReturnHelpers
    {
        //TODO: return type of 'object' in Tuples should be converted to '?' in C# 8

        public static T ToDisplay<T>(T obj) 
        {
            return obj;
        }

        public static (T, object, string) ToDisplay<T>(T toDisplay, string withMessage)
        {
            return (toDisplay, null, withMessage);
        }

        public static (object,T) ToDisplayAndPersist<T>(T toDisplay)
        {
            return (null, toDisplay);
        }

        public static (object,T, string) ToDisplayAndPersist<T>(T toDisplayAndPersist, string withMessage)
        {
            return (null, toDisplayAndPersist, withMessage);
        }

        public static (T, U) DisplayAndPersistDifferentItems<T, U>(T toDisplay, U toPersist)
        {
            return (toDisplay, toPersist);
        }

        public static (T, U, string) DisplayAndPersistDifferentItems<T,U>(T toDisplay, U toPersist, string withMessage)
        {
            return (toDisplay, toPersist, withMessage);
        }
    }
}
