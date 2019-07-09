using NakedObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AdventureWorksModel {

    public class BusinessEntity : IBusinessEntity {
        public BusinessEntity(
            int businessEntityID,
            Guid businessEntityRowguid,
            DateTime businessEntityModifiedDate
            )
        {
            BusinessEntityID = businessEntityID;
            BusinessEntityRowguid = businessEntityRowguid;
            BusinessEntityModifiedDate = businessEntityModifiedDate;
        }

        public BusinessEntity() {}

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid BusinessEntityRowguid { get; set; }

        [Hidden(WhenTo.Always)]
        [ConcurrencyCheck]
        public virtual DateTime BusinessEntityModifiedDate { get; set; }

        private ICollection<BusinessEntityAddress> _addresses = new List<BusinessEntityAddress>();

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false,
            nameof(BusinessEntityAddress.AddressType),
            nameof(BusinessEntityAddress.Address))] 
        public virtual ICollection<BusinessEntityAddress> Addresses
        {
            get { return _addresses; }
            set { _addresses = value; }
        }

        private ICollection<BusinessEntityContact> _contacts = new List<BusinessEntityContact>();

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false, "ContactType", "Person")] 
        public virtual ICollection<BusinessEntityContact> Contacts {
            get { return _contacts; }
            set { _contacts = value; }
        }


    }

    public static class BusinessEntityFunctions
    {
        public static bool HideContacts(BusinessEntity be)
        {
            return false;
        }

        //TODO: This needs modification to create persisted address with all fields filled.
        public static Address CreateNewAddress(BusinessEntity be)
        {
            var a = new Address();  //TODO add all fields
            //a.AddressFor = be;
            return a;
        }


        #region Life Cycle Methods
        public static BusinessEntity Persisting(BusinessEntity be, [Injected] Guid guid, [Injected] DateTime now)
        {
            return Updating(be, now).With(x => x.BusinessEntityRowguid, guid);
        }

        public static BusinessEntity Updating(BusinessEntity be, [Injected] DateTime now)
        {
            return be.With(x => x.BusinessEntityModifiedDate, now);
        }
        #endregion
    }
}
