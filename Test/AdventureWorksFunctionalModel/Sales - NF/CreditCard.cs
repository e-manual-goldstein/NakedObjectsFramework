// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel {
        public record CreditCard {
        [Hidden]
        public virtual int CreditCardID { get; init; }

        [MemberOrder(1)]
        public virtual string CardType { get; init; }

        [MemberOrder(2)][DescribedAs("Without spaces")]
        public virtual string CardNumber { get; init; }

        //TODO: what was intended purpose?
        //private string _ObfuscatedNumber;

        
        [MemberOrder(3)]
        public virtual byte ExpMonth { get; init; }

        [MemberOrder(4)]
        public virtual short ExpYear { get; init; }

        private ICollection<PersonCreditCard> _links = new List<PersonCreditCard>();
        private Person p;

        public CreditCard(Person p)
        {
            this.p = p;
        }

        [Named("Persons")]
        [MemberOrder(5)]
        //[TableOrder(True, "Contact")]
        public virtual ICollection<PersonCreditCard> PersonLinks {
            get { return _links; }
            set { _links = value; }
        }

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }


        public override string ToString() => CreditCard_Functions.ObfuscatedNumber(this);


        [Hidden]
        
        public ICreditCardCreator Creator { get; init; }

        
        public Person ForContact { get; init; }

    }
}