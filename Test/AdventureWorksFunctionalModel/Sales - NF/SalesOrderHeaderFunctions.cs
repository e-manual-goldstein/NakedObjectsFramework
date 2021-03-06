// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions;

namespace AdventureWorksModel {
    [Named("Sales Order")]
        public  static class SalesOrderHeaderFunctions { 

        #region Actions

        #region Add New Detail

        [DescribedAs("Add a new line item to the order")]
#pragma warning disable 612,618
        [MemberOrder(2, "Details")]
#pragma warning restore 612,618
        public static (SalesOrderHeader, IContext) AddNewDetail(
                this SalesOrderHeader soh,
                Product product,
                [DefaultValue((short) 1), Range(1, 999)] short quantity,
                IContext context
            ) {
            int stock = product.NumberInStock();
            var sod = new SalesOrderDetail()
            {
                SalesOrderHeader = soh,
                SalesOrderID = soh.SalesOrderID,
                OrderQty = quantity,
                SpecialOfferProduct = Product_Functions.BestSpecialOfferProduct(product, quantity, context)
            };
            //TODO:
            //sod.Recalculate();
            return (soh, context.WithPendingSave(sod).WithWarnUser(stock < quantity ? $"Current inventory of {product} is {stock}" : ""));
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0AddNewDetail(this SalesOrderHeader soh, [Range(2,0)] string name, IContext context) => 
            Product_MenuFunctions.FindProductByName( name, context);

        #endregion

        //        #region Add New Details
        //        [DescribedAs("Add multiple line items to the order")]
        //        [MultiLine()]
        //#pragma warning disable 612, 618
        //        [MemberOrder(1, "Details")]
        //#pragma warning restore 612,618
        //        public void AddNewDetails(
        //            Product product,
        //           [DefaultValue((short)1)] short quantity,
        //           IQueryable<SpecialOfferProduct> sops)
        //        {
        //            var detail = AddNewDetail(product, quantity, sops);
        //            Container.Persist(ref detail);
        //        }
        //        public virtual string DisableAddNewDetails()
        //        {
        //            return DisableAddNewDetail();
        //        }
        //        [PageSize(20)]
        //        public IQueryable<Product> AutoComplete0AddNewDetails([Range(2,0)] string name, IQueryable<Product> products)
        //        {
        //            return AutoComplete0AddNewDetail(name, products);
        //        }

        //        public string ValidateAddNewDetails(short quantity)
        //        {
        //            var rb = new ReasonBuilder();
        //            rb.AppendOnCondition(quantity <= 0, "Must be > 0");
        //            return rb.Reason;
        //        }

        //        #endregion

        //        #region Remove Detail
        //        public void RemoveDetail(SalesOrderDetail detailToRemove) {
        //            if (Details.Contains(detailToRemove)) {
        //                Details.Remove(detailToRemove);
        //            }
        //        }

        //        public IEnumerable<SalesOrderDetail> Choices0RemoveDetail() {
        //            return Details;
        //        }

        //        public SalesOrderDetail Default0RemoveDetail() {
        //            return Details.FirstOrDefault();
        //        }

        //        [MemberOrder(3)]
        //        public void RemoveDetails(this IEnumerable<SalesOrderDetail> details) {
        //            foreach (SalesOrderDetail detail in details) {
        //                if (Details.Contains(detail)) {
        //                    Details.Remove(detail);
        //                }
        //            }
        //        }
        //        #endregion

        //        #region AdjustQuantities
        //        [MemberOrder(4)]
        //        public void AdjustQuantities(this IEnumerable<SalesOrderDetail> details, short newQuantity)
        //        {
        //            foreach (SalesOrderDetail detail in details)
        //            {
        //                detail.OrderQty = newQuantity;
        //            }
        //        }

        //        public string ValidateAdjustQuantities(IEnumerable<SalesOrderDetail> details, short newQuantity)
        //        {
        //            var rb = new ReasonBuilder();
        //            rb.AppendOnCondition(details.Count(d => d.OrderQty == newQuantity) == details.Count(),
        //                "All selected details already have specified quantity");
        //            return rb.Reason;
        //        }

        //        #endregion

        //        #region CreateNewCreditCard

        //        [Hidden]
        //        public void CreatedCardHasBeenSaved(CreditCard card) {
        //            CreditCard = card;
        //        }

        //        #endregion

        //        #region AddNewSalesReason

        //#pragma warning disable 618
        //        [MemberOrder(Name= "SalesOrderHeaderSalesReason", Sequence = "1")]
        //#pragma warning restore 618
        //        public void AddNewSalesReason(SalesReason reason) {
        //            if (SalesOrderHeaderSalesReason.All(y => y.SalesReason != reason)) {
        //                var link = Container.NewTransientInstance<SalesOrderHeaderSalesReason>();
        //                link.SalesOrderHeader = this;
        //                link.SalesReason = reason;
        //                Container.Persist(ref link);
        //                SalesOrderHeaderSalesReason.Add(link);
        //            }
        //            else {
        //                Container.WarnUser(string.Format("{0} already exists in Sales Reasons", reason.Name));
        //            }
        //        }

        //        public string ValidateAddNewSalesReason(SalesReason reason) {
        //            return SalesOrderHeaderSalesReason.Any(y => y.SalesReason == reason) ? string.Format("{0} already exists in Sales Reasons", reason.Name) : null;
        //        }

        //        [MemberOrder(1)]
        //        public void RemoveSalesReason(SalesOrderHeaderSalesReason reason)
        //        {
        //            this.SalesOrderHeaderSalesReason.Remove(reason);
        //            Container.DisposeInstance(reason);
        //        }


        //        public IList<SalesOrderHeaderSalesReason> Choices0RemoveSalesReason()
        //        {
        //            return this.SalesOrderHeaderSalesReason.ToList();
        //        }

        //        [MemberOrder(2)]
        //        public void AddNewSalesReasons(IEnumerable<SalesReason> reasons) {
        //            foreach (SalesReason r in reasons) {
        //                AddNewSalesReason(r);
        //            }
        //        }

        //        public string ValidateAddNewSalesReasons(IEnumerable<SalesReason> reasons) {
        //            return reasons.Select(ValidateAddNewSalesReason).Aggregate("", (s, t) => string.IsNullOrEmpty(s) ? t : s + ", " + t);
        //        }

        //        [MemberOrder(2)]
        //        public void RemoveSalesReasons(
        //            this IEnumerable<SalesOrderHeaderSalesReason> salesOrderHeaderSalesReason)
        //        {
        //            foreach(var reason in salesOrderHeaderSalesReason)
        //            {
        //                this.RemoveSalesReason(reason);
        //            }
        //        }

        //        // This is done with an enum in order to test enum parameter handling by the framework
        //        public enum SalesReasonCategories {
        //            Other,
        //            Promotion,
        //            Marketing
        //        }

        //        [MemberOrder(3)]
        //        public void AddNewSalesReasonsByCategory(SalesReasonCategories reasonCategory) {
        //            IQueryable<SalesReason> allReasons = Container.Instances<SalesReason>();
        //            var catName = reasonCategory.ToString();

        //            foreach (SalesReason salesReason in allReasons.Where(salesReason => salesReason.ReasonType == catName)) {
        //                AddNewSalesReason(salesReason);
        //            }
        //        }

        //        [MemberOrder(4)]
        //        public void AddNewSalesReasonsByCategories(IEnumerable<SalesReasonCategories> reasonCategories) {
        //            foreach (SalesReasonCategories rc in reasonCategories) {
        //                AddNewSalesReasonsByCategory(rc);
        //            }
        //        }

        //        // Use 'hide', 'dis', 'val', 'actdef', 'actcho' shortcuts to add supporting methods here.

        //        #endregion

        //        #region ApproveOrder

        //        [MemberOrder(1)]
        //        public void ApproveOrder() {
        //            Status = (byte) OrderStatus.Approved;
        //        }

        //        public virtual bool HideApproveOrder() {
        //            return !IsInProcess();
        //        }

        //        public virtual string DisableApproveOrder() {
        //            var rb = new ReasonBuilder();
        //            if (Details.Count == 0) {
        //                rb.Append("Cannot approve orders with no Details");
        //            }
        //            return rb.Reason;
        //        }

        //        #endregion

        //        #region MarkAsShipped

        //        [DescribedAs("Indicate that the order has been shipped, specifying the date")]
        //        [Hidden] //Testing that the complementary methods don't show up either
        //        public void MarkAsShipped(DateTime shipDate) {
        //            Status = (byte) OrderStatus.Shipped;
        //            ShipDate = shipDate;
        //        }

        //        public virtual string ValidateMarkAsShipped(DateTime date) {
        //            var rb = new ReasonBuilder();
        //            if (date.Date > DateTime.Now.Date) {
        //                rb.Append("Ship Date cannot be after Today");
        //            }
        //            return rb.Reason;
        //        }

        //        public virtual string DisableMarkAsShipped() {
        //            if (!IsApproved()) {
        //                return "Order must be Approved before shipping";
        //            }
        //            return null;
        //        }

        //        public virtual bool HideMarkAsShipped() {
        //            return IsRejected() || IsShipped() || IsCancelled();
        //        }

        //        #endregion

        //        #region CancelOrder

        //        // return this for testing purposes
        //        public SalesOrderHeader CancelOrder() {
        //            Status = (byte) OrderStatus.Cancelled;
        //            return this;
        //        }

        //        public virtual bool HideCancelOrder() {
        //            return IsShipped() || IsCancelled();
        //        }

        //        #endregion

        #endregion

        internal static bool IsInProcess(this SalesOrderHeader soh)
        {
            return soh.Status.Equals((byte)OrderStatus.InProcess);
        }

        internal static bool IsApproved(this SalesOrderHeader soh)
        {
            return soh.Status.Equals((byte)OrderStatus.Approved);
        }

        internal static bool IsBackOrdered(this SalesOrderHeader soh)
        {
            return soh.Status.Equals((byte)OrderStatus.BackOrdered);
        }

        internal static bool IsRejected(this SalesOrderHeader soh)
        {
            return soh.Status.Equals((byte)OrderStatus.Rejected);
        }

        internal static bool IsShipped(this SalesOrderHeader soh)
        {
            return soh.Status.Equals((byte)OrderStatus.Shipped);
        }

        internal static bool IsCancelled(this SalesOrderHeader soh)
        {
            return soh.Status.Equals((byte)OrderStatus.Cancelled);
        }


        public static ShipMethod DefaultShipMethod(this SalesOrderHeader soh, IContext context) => context.Instances<ShipMethod>().FirstOrDefault();


        public static string DisableDueDate(this SalesOrderHeader soh)
        {
            return soh.DisableIfShipped();
        }

        public static string ValidateDueDate(this SalesOrderHeader soh, DateTime dueDate)
        {
            if (dueDate.Date < soh.OrderDate.Date)
            {
                return "Due date cannot be before order date";
            }

            return null;
        }

        private static string DisableIfShipped(this SalesOrderHeader soh)
        {
            if (soh.IsShipped())
            {
                return "Order has been shipped";
            }
            return null;
        }

        public static string DisableShipDate(this SalesOrderHeader soh) => soh.DisableIfShipped();

        public static string DisableRecalculate(this SalesOrderHeader soh) =>
            !soh.IsInProcess() ? "Can only recalculate an 'In Process' order" : null;

        public static string ValidateShipDate(this SalesOrderHeader soh, DateTime? shipDate)
        {
            if (shipDate.HasValue && shipDate.Value.Date < soh.OrderDate.Date)
            {
                return "Ship date cannot be before order date";
            }

            return null;
        }

        public static string DisableAddNewDetail(this SalesOrderHeader soh)
        {
            if (!soh.IsInProcess())
            {
                return "Can only add to 'In Process' order";
            }
            return null;
        }
    }

}