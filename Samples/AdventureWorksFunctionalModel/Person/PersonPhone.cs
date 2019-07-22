using NakedObjects;
using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel
{
    public class PersonPhone : IHasModifiedDate {

        public PersonPhone(
            int businessEntityID, 
            Person person, 
            PhoneNumberType phoneNumberType, 
            int phoneNumberTypeID, 
            string phoneNumber)
        {
            BusinessEntityID = businessEntityID;
            Person = person;
            PhoneNumberType = phoneNumberType;
            PhoneNumberTypeID = phoneNumberTypeID;
            PhoneNumber = phoneNumber;
        }

        public PersonPhone() { }

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        public virtual string PhoneNumber { get; set; }

        [NakedObjectsIgnore]
        public virtual int PersonID { get; set; }

        [NakedObjectsIgnore]
        public virtual Person Person { get; set; }

        [NakedObjectsIgnore]
        public virtual int PhoneNumberTypeID { get; set; }

        public virtual PhoneNumberType PhoneNumberType { get; set; }

        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }

    public static class PersonPhoneFunctions
    {

        public static PersonPhone Persisting(PersonPhone pp,  [Injected] DateTime now)
        {
            return Updating(pp, now);
        }

        public static PersonPhone Updating(PersonPhone pp, [Injected] DateTime now)
        {
            return pp.UpdateModifiedDate(now);
        }

        public static string Title(this PersonPhone pp)
        {
           return pp.CreateTitle($"{pp.PhoneNumberType}:{pp.PhoneNumber}");
        }
    }
}
