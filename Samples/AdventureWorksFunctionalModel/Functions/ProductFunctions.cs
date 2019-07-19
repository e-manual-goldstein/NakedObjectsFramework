// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AdventureWorksModel;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksFunctionalModel.Functions {
    public static class ProductFunctions {
        [QueryOnly]
        public static IProduct GetAnotherProduct(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            return allProducts.First(p => p.ProductID != product.ProductID);
        }

        [QueryOnly]
        public static IQueryable<IProduct> GetProducts(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            return  allProducts.Where(p => p.ProductID != product.ProductID).Take(2);
        }


        [QueryOnly]
        public static (object, Product) GetAndPersistProduct(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return Result.ToPersistAndDisplay(pp);
        }
        public static (object, Product) UpdateProductUsingRemute(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);

            var up = pp.With(x => x.Name, $"{pp.Name}:1");
            return Result.ToPersistAndDisplay(up);
        }

        [QueryOnly]
        public static (object, IProduct) UpdateIProductUsingRemute(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);

            var up = pp.With(x => x.Name, $"{pp.Name}:1");
            return Result.ToPersistAndDisplay(up);
        }

        [QueryOnly]
        public static Product GetAndChangeButNotPersistProduct(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:2";
            return pp;
        }
    }
}