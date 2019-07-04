// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity.Core.Mapping;
using NakedObjects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AdventureWorksModel;
using Remutable;

namespace AdventureWorksFunctionalModel.Functions {

    public static class CloneHelpers {





        public static TInstance With<TInstance, TValue>(
            this TInstance source,
            Expression<Func<TInstance, TValue>> expression,
            TValue value) {

            var instanceType = typeof(TInstance);

            if (instanceType.IsInterface)
            {
                // get matching impl type by convention for the moment 
                var implTypeName = $"{instanceType.Namespace}.{instanceType.Name.Remove(0, 1)}";
                var implType = instanceType.Assembly.GetType(implTypeName);
                instanceType = implType;
            }

            var cc = instanceType.GetConstructors().Single(c => c.GetParameters().Any<ParameterInfo>());

            var config = new ActivationConfiguration().Configure(cc);

            var rm = new Remute(config);

            return rm.With(source, expression, value);
        }
    }


    public static class ProductFunctions {

        [QueryOnly]
        public static IProduct GetAnotherProduct(this Product product, IQueryable<IProduct> allProducts) {
            return allProducts.First(p => p.ProductID != product.ProductID);
        }

        [QueryOnly]
        public static Tuple<Product, Product> GetAndPersistProduct(this Product product, IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return new Tuple<Product, Product>(pp, pp);
        }

        [QueryOnly]
        public static Tuple<Product, Product> UpdateProductUsingRemute(this Product product, IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);

            var up = pp.With(x => x.Name, $"{pp.Name}:1");
            return new Tuple<Product, Product>(up, up);
        }


        [QueryOnly]
        public static Tuple<IProduct, IProduct> UpdateIProductUsingRemute(this Product product, IQueryable<IProduct> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);

            var up = pp.With(x => x.Name, $"{pp.Name}:1");
            return new Tuple<IProduct, IProduct>(up, up);
        }

        
        [QueryOnly]
        public static Product GetAndChangeButNotPersistProduct(this Product product, IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:2";
            return pp;
        }
    }
}