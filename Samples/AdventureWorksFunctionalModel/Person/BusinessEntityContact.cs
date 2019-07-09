using NakedObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    [DisplayName("Contact")]
    public partial class BusinessEntityContact {

        //TODO: Ensure constructor includes all properties
        public BusinessEntityContact(int businessEntityId, int personIf, int contactTypeID)
        {
            BusinessEntityID = businessEntityId;
            PersonID = personIf;
            ContactTypeID = contactTypeID;
        }
        public BusinessEntityContact()
        {

        }

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }
        [NakedObjectsIgnore]
        public virtual int PersonID { get; set; }
        [NakedObjectsIgnore]
        public virtual int ContactTypeID { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        [NakedObjectsIgnore]
        public virtual BusinessEntity BusinessEntity { get; set; }
        public virtual ContactType ContactType { get; set; }
        public virtual Person Person { get; set; }
    }

    public static class BusinessEntityContactFunctions
    {
        public static string Title(BusinessEntityContact bec)
        {
            return ContactTypeFunctions.Title(bec.ContactType) + ":" + PersonFunctions.Title(bec.Person);
        }

        public static BusinessEntityContact Persisting(BusinessEntityContact bec, [Injected] Guid guid, [Injected] DateTime now)
        {
            return Updating(bec, now).With(x => x.rowguid, guid);
        }

        public static BusinessEntityContact Updating(BusinessEntityContact bec, [Injected] DateTime now)
        {
            return bec.With(x => x.ModifiedDate, now);
        }
    }
}
