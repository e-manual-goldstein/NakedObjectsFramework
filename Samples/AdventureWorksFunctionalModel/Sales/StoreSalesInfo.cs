using NakedFunctions;
using NakedObjects;
using System;
using System.Linq;

namespace AdventureWorksModel
{
    //TODO: Need to think how we want to do ViewModels. Can't require methods to be implemented.
    //Probably just need a constructor that takes all keys, as well as any required Injected params
    public class StoreSalesInfo // :  IViewModelSwitchable
    {
            var t = Container.NewTitleBuilder();
        [MemberOrder(1), Disabled]
        public virtual string AccountNumber { get; set; }


        public bool EditMode { get; set; }

    }

    //public static class StoreSalesInfoFunctions {

    //    public static string Title(
    //        StoreSalesInfo ssi,
    //        [Injected] IQueryable<Customer> customers)
    //    {
    //        return $"{(ssi.EditMode ? "Editing - ":"")} Sales Info for: {StoreName(ssi, customers)}";
    //    }

    //    #region ViewModel functions
    //    public static string[] DeriveKeys(StoreSalesInfo ssi)
    //    {
    //        return new string[] { AccountNumber, EditMode.ToString() };
    //    }

    //    public void PopulateUsingKeys(
    //        string[] keys)
    //    {
    //        AccountNumber = keys[0];
    //        EditMode = bool.Parse(keys[1]);
    //    }

    //    #endregion

    //    #region Actions displayed as properties 
    //    //StoreName = cus.Store.Name;
    //    //SalesPerson = cus.Store.SalesPerson;

    //    [DisplayAsProperty]
    //        public static SalesTerritory SalesTerritory(
    //            this StoreSalesInfo ssi,
    //            [Injected] IQueryable<Customer> customers)
    //    {
    //        return GetCustomer(ssi, customers).SalesTerritory;
    //    }

    //    [DisplayAsProperty]
    //    public static string StoreName(
    //        this StoreSalesInfo ssi,
    //        [Injected] IQueryable<Customer> customers)
    //    {
    //        return GetCustomer(ssi, customers).Store.Name;
    //    }

    //    [DisplayAsProperty]
    //    public static SalesPerson SalesPerson(
    //        this StoreSalesInfo ssi,
    //        [Injected] IQueryable<Customer> customers)
    //    {
    //        return GetCustomer(ssi, customers).Store.SalesPerson;
    //    }

    //    private static Customer GetCustomer(StoreSalesInfo ssi,
    //            [Injected] IQueryable<Customer> customers)
    //    {
    //       return CustomerRepository.FindCustomerByAccountNumber(ssi.AccountNumber, customers).Item1;
    //    }

    //    #endregion

    //    public static StoreSalesInfo Edit(StoreSalesInfo ssi)
    //    {
    //        return ssi.With(x => x.EditMode,true);
    //    }


    //    public static bool HideEdit(StoreSalesInfo ssi)
    //    {
    //        return IsEditView(ssi);
    //    }

    //    public static bool IsEditView(StoreSalesInfo ssi)
    //    {
    //        return ssi.EditMode;
    //    }

    //    public static (StoreSalesInfo, Customer) Save(
    //        StoreSalesInfo ssi,
    //        [Injected] IQueryable<Customer> customers)
    //    {
    //        //TODO: This is pretty horrible, and needs a rethink!
    //        var ssi2 = ssi.With(x => x.EditMode, false);
    //        var (cus, _) = CustomerRepository.FindCustomerByAccountNumber(ssi.AccountNumber, customers);
    //        var st = SalesTerritory(ssi, customers);
    //        var sp = SalesPerson(ssi, customers);
    //        var sn = StoreName(ssi, customers);
    //        var cus2 = cus.With(c => c.SalesTerritory, st)
    //            .With(c => c.Store.SalesPerson, sp)
    //            .With(c => c.Store.Name, sn);
    //        return (ssi2, cus2);
    //    }


    //    public static bool HideSave(
    //        StoreSalesInfo ssi)
    //    {
    //        return !IsEditView(ssi);
    //    }


}
