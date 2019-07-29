using NakedFunctions;
using NakedObjects;
using static NakedFunctions.Result;
using System.Linq;

namespace AdventureWorksModel
{
    //TODO: Need to think how we want to do ViewModels. Can't require methods to be implemented.
    //Probably just need a constructor that takes all keys, as well as any required Injected params
    public class StoreSalesInfo: IFunctionalVMSwitchable
    {
        public StoreSalesInfo(
            string accountNumber, 
            bool editMode,
            string storeName,
            SalesTerritory salesTerritory,
            SalesPerson salesPerson)
        {
            AccountNumber = accountNumber;
            StoreName = storeName;
            SalesTerritory = salesTerritory;
            SalesPerson = SalesPerson;
            EditMode = editMode;
        }

        public StoreSalesInfo()
        {

        }

        [MemberOrder(1), Disabled]
        public  string AccountNumber { get; set; }

        [MemberOrder(2)]
        public  string StoreName { get; set; }

        [MemberOrder(3)]
        public  SalesTerritory SalesTerritory { get; set; }

        [MemberOrder(4)]
        public  SalesPerson SalesPerson { get; set; }


        public bool EditMode { get; set; }

    }

    public static class StoreSalesInfoFunctions
    {

        public static string Title(
            StoreSalesInfo vm,
            [Injected] IQueryable<Customer> customers)
        {
            return $"{(vm.EditMode ? "Editing - " : "")} Sales Info for: {vm.StoreName}";
        }


        public static string[] DeriveKeys(StoreSalesInfo vm)
        {
            return new string[] { vm.AccountNumber, vm.EditMode.ToString() };
        }

        public static StoreSalesInfo PopulateUsingKeys(
            StoreSalesInfo vm,
            string[] keys,
            [Injected] IQueryable<Customer> customers)
        {
            var cus = CustomerRepository.FindCustomerByAccountNumber(keys[0], customers).Item1;
            return vm.With(x => x.AccountNumber, keys[0])
                .With(x => x.SalesTerritory, cus.SalesTerritory)
                .With(x => x.SalesPerson, cus.Store.SalesPerson)
                .With(x => x.EditMode, bool.Parse(keys[1]));
        }

        public static StoreSalesInfo Edit(StoreSalesInfo ssi)
        {
            return ssi.With(x => x.EditMode, true);
        }


        public static bool HideEdit(StoreSalesInfo ssi)
        {
            return IsEditView(ssi);
        }

        public static bool IsEditView(StoreSalesInfo ssi)
        {
            return ssi.EditMode;
        }

        public static (Customer, Customer) Save(
            StoreSalesInfo vm,
            [Injected] IQueryable<Customer> customers)
        {
            var (cus, _) = CustomerRepository.FindCustomerByAccountNumber(vm.AccountNumber, customers);
            var st = vm.SalesTerritory;
            var sp = vm.SalesPerson;
            var sn = vm.StoreName;
            var cus2 = cus
                .With(c => c.SalesTerritory, st)
                .With(c => c.Store.SalesPerson, sp)
                .With(c => c.Store.Name, sn);
            return DisplayAndPersist(cus2);
        }


        public static bool HideSave(
            StoreSalesInfo ssi)
        {
            return !IsEditView(ssi);
        }
    }
}
