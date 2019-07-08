using NakedObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel
{
    [DisplayName("Address")]
    public class BusinessEntityAddress
    {

        public BusinessEntityAddress(int addressID, int addressTypeID, int businessEntityID, AddressType addressType, Guid guid, DateTime now)
        {
            AddressID = addressID;
            AddressTypeID = addressTypeID;
            BusinessEntityID = businessEntityID;
            AddressType = addressType;
            rowguid = guid;
            ModifiedDate = now;
        }

        public BusinessEntityAddress()
        {

        }

        [Disabled]
        public virtual int BusinessEntityID { get; set; }

        [Disabled]
        public virtual int AddressID { get; set; }

        [Disabled]
        public virtual int AddressTypeID { get; set; }
        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        [MemberOrder(1)]
        [Optionally]
        public virtual AddressType AddressType { get; set; }

        [MemberOrder(2)]
        [Optionally]
        public virtual Address Address { get; set; }

        [MemberOrder(3)]
        public virtual BusinessEntity BusinessEntity { get; set; }
    }

    public static class BusinessEntityAddressFunctions {
        
        public static BusinessEntityAddress Persisting(BusinessEntityAddress a, [Injected] Guid guid, [Injected] DateTime now)
    {
        return Updating(a, now).With(x => x.rowguid, guid);
    }

    public static BusinessEntityAddress Updating(BusinessEntityAddress a, [Injected] DateTime now)
    {
        return a.With(x => x.ModifiedDate, now);
    }

    public static string Title(BusinessEntityAddress a)
    {
            return AddressTypeFunctions.Title(a.AddressType) + ":" + AddressFunctions.Title(a.Address);
    }

}
}
