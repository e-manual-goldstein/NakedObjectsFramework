using System;

namespace NakedFunctions
{
    //TODO:put here temporarily, pending transfer to NakedFunctionsProgrammingModel.Helpers
    /// <summary>
    /// Helper functions for returning items from action functions
    /// </summary>
    /// 
    public static class Result
    {
        //TODO: return type of 'object' in Tuples should be converted to '?' in C# 8
        public static (object, object, string) MessageOnly(string message)
        {
            return (null, null, message);
        }


        public static T ToDisplay<T>(T item) 
        {
            return item;
        }

        public static (T, object, string) ToDisplay<T>(T item, string withMessage)
        {
            return (item, null, withMessage);
        }

        public static (object,T) ToPersistAndDisplay<T>(T item) 
        {
            return (null, item);
        }

        public static (object,T, string) ToPersistAndDisplay<T>(T item, string withMessage)
        {
            return (null, item, withMessage);
        }

        public static (T, U) ToDisplayAndPersistDifferentItems<T, U>(T itemToDisplay, U toPersist)
        {
            return (itemToDisplay, toPersist);

        }

        public static (T, U, string) ToDisplayAndPersistDifferentItems<T,U>(T itemToDisplay, U toPersist, string withMessage)
        {
            return (itemToDisplay, toPersist, withMessage);
        }
    }
}
