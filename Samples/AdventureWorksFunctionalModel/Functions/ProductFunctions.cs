// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Security.Principal;
using AdventureWorksModel;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksFunctionalModel.Functions {
    public static class ProductFunctions {
        [QueryOnly]
        public static IProduct GetAnotherProduct(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            return allProducts.First(p => p.ProductID != product.ProductID);
        }

        //public static string DisableGetAnotherProduct(this Product product, [Injected]IQueryable<IProduct> products) {
        //    return "Always disabled";
        //}

        public static string ValidateGetAnotherProduct(this Product product, [Injected]IQueryable<IProduct> products)
        {
            return "Always invalid";
        }


        [QueryOnly]
        public static (IProduct, string) GetAnotherProductWithWarning(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            return (allProducts.First(p => p.ProductID != product.ProductID), "A warning message");
        }

        [QueryOnly]
        public static IQueryable<IProduct> GetProducts(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            return allProducts.Where(p => p.ProductID != product.ProductID).Take(2);
        }

        [QueryOnly]
        public static (Product, Product) GetAndPersistProduct(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return Result.DisplayAndPersist(pp);
        }


        [QueryOnly]
        public static (Product, Product[]) GetAndPersistProducts(this Product product, [Injected] IQueryable<Product> allProducts)
        {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return (pp, new[] {pp});
        }

        [QueryOnly]
        public static (Product, Product[], string) GetAndPersistProductsWithWarning(this Product product, [Injected] IQueryable<Product> allProducts)
        {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return (pp, new[] { pp }, "A warning message");
        }


        [QueryOnly]
        public static (Product, Product, string) GetAndPersistProductWithWarning(this Product product, [Injected] IQueryable<Product> allProducts)
        {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return Result.DisplayAndPersist(pp, "A warning message");
        }


        public static (Product, Product) UpdateProductUsingRemute(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);

            var up = pp.With(x => x.Name, $"{pp.Name}:1");
            return Result.DisplayAndPersist(up);
        }

        [QueryOnly]
        public static (IProduct, IProduct) UpdateIProductUsingRemute(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);

            var up = pp.With(x => x.Name, $"{pp.Name}:1");
            return Result.DisplayAndPersist(up);
        }

        [QueryOnly]
        public static Product GetAndChangeButNotPersistProduct(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:2";
            return pp;
        }

        [QueryOnly]
        public static IProduct TestInjectedGuid(this Product product, [Injected] Guid guid) {
            var test = guid;
            return product;
        }

        [QueryOnly]
        public static IProduct TestInjectedPrincipal(this Product product, [Injected] IPrincipal principal) {
            var test = principal;
            return product;
        }

        [QueryOnly]
        public static IProduct TestInjectedDateTime(this Product product, [Injected] DateTime dateTime) {
            var test = dateTime;
            return product;
        }

        [QueryOnly]
        public static IProduct TestInjectedRandom(this Product product, [Injected] int random) {
            var test = random;
            return product;
        }

        public static string Title(this Product product)
        {
            return "A Title from title function";
        }

        public static IProduct Persisting(this Product product, [Injected] IQueryable<Product> allProducts, [Injected] Guid guid) {
            return product;
        }

        public static IProduct Persisted(this Product product, [Injected] IQueryable<Product> allProducts, [Injected] Guid guid) {
            return null;
        }

        public static IProduct Updating(this Product product, [Injected] IQueryable<Product> allProducts, [Injected] Guid guid) {
            return product;
        }

        public static IProduct Updated(this Product product, [Injected] IQueryable<Product> allProducts, [Injected] Guid guid) {
            return null;
        }
    }
}