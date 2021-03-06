// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFunctions;

namespace AdventureWorksModel {
        public record SalesOrderDetail {
        #region Injected Services
        
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        #region OrderQty

        
        [MemberOrder(15)]
        public virtual short OrderQty { get; set; }

        #endregion

        #region UnitPrice

        
        [MemberOrder(20)]
        [Mask("C")]
        public virtual decimal UnitPrice { get; set; }

        #endregion

        #region UnitPriceDiscount

        
        [Named("Discount")]
        [MemberOrder(30)]
        [Mask("C")]
        public virtual decimal UnitPriceDiscount { get; set; }

        #endregion

        #region LineTotal
        
        
        [MemberOrder(40)]
        [Mask("C")]
        public virtual decimal LineTotal { get; set; }

        #endregion

        #region CarrierTrackingNumber

        
        [MemberOrder(50)]
        
        public virtual string CarrierTrackingNumber { get; set; }

        #endregion

        #region SalesOrder

        // [Hidden]
        public virtual SalesOrderHeader SalesOrderHeader { get; set; }

        #endregion

        public override string ToString() => $"{OrderQty} x {Product}";




        #region ID

        [Hidden]
        public virtual int SalesOrderID { get; set; }

        [Hidden]
        public virtual int SalesOrderDetailID { get; set; }

        #endregion

        #region Product & Special Offer

        #region SpecialOfferProduct

        [Hidden]
        public virtual int SpecialOfferID { get; set; }

        [Hidden]
        public virtual int ProductID { get; set; }

        [Hidden]
        public virtual SpecialOfferProduct SpecialOfferProduct { get; set; }

        #endregion

        
        
        [MemberOrder(11)]
        public virtual Product Product {
            get { return SpecialOfferProduct == null ? null : SpecialOfferProduct.Product; }
            set {
                //Does nothing -  derived field
            }
        }

        
        [MemberOrder(12)]
        public virtual SpecialOffer SpecialOffer {
            get { return SpecialOfferProduct == null ? null : SpecialOfferProduct.SpecialOffer; }
            set { }
        }

        #endregion

       [MemberOrder(99),ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        [Hidden]
        public virtual Guid rowguid { get; set; }
    }

    public static class SalesOrderDetail_Functions
    {
        #region Life Cycle methods

        public static void Persisted(this SalesOrderDetail sod)
        {
            //TODO:
            //SalesOrderHeader.Details.Add(this);
            //SalesOrderHeader.Recalculate();
        }

        #endregion

        public static (SalesOrderDetail, IContext) Recalculate(this SalesOrderDetail sod)
        {
            throw new NotImplementedException();
            //UnitPrice = SpecialOfferProduct.Product.ListPrice;
            //UnitPriceDiscount = (SpecialOfferProduct.SpecialOffer.DiscountPct * UnitPrice);
            //LineTotal = (UnitPrice - UnitPriceDiscount) * OrderQty;
            //SalesOrderHeader.Recalculate();
        }

        public static (SalesOrderDetail, IContext) ChangeQuantity(this SalesOrderDetail sod, short newQuantity, IContext context)
        {
            throw new NotImplementedException();
            //OrderQty = newQuantity;
            //            IQueryable<SpecialOfferProduct> sops
            //SpecialOfferProduct = ProductFunctions2.BestSpecialOfferProduct(Product, newQuantity, sops);
            //Recalculate();
        }

        public static string DisableChangeQuantity()
        {
            throw new NotImplementedException();
            //return SalesOrderHeader.DisableAddNewDetail();
        }
    }
}