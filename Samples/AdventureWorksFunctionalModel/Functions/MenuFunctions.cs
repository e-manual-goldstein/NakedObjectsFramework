// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using AdventureWorksModel;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksFunctionalModel.Functions {


    public static class MenuFunctions {
        [QueryOnly]
        public static Product GetRandomProduct(MainMenu ph, [Injected] IQueryable<Product> allProducts) {
            int count = new Random().Next(allProducts.Count());
            var p = allProducts.OrderBy(n => "").Skip(count).FirstOrDefault();
            return p;
        }

        [QueryOnly]
        public static Product GetProductById(MainMenu ph, [Injected] IQueryable<Product> allProducts, int id) {
            var p = allProducts.Single(x => x.ProductID == id);
            return p;
        }
    }
}