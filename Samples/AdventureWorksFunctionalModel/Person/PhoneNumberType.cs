using NakedObjects;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    [Bounded]
    public class PhoneNumberType {
        public PhoneNumberType(int phoneNumberTypeID, string name, DateTime modifiedDate)
        {
            PhoneNumberTypeID = phoneNumberTypeID;
            Name = name;
            ModifiedDate = modifiedDate;
        }

        public PhoneNumberType() { }

        [NakedObjectsIgnore]
        public virtual int PhoneNumberTypeID { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual string Name { get; set; }

        [NakedObjectsIgnore]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }
    public static class PhoneNumberTypeFunctions
    {
        public static string Title(PhoneNumberType pnt)
        {
            return pnt.Name;
        }
    }
}
