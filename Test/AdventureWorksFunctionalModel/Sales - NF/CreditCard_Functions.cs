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
        public static class CreditCard_Functions {
        
        #region Life Cycle Methods
        public static CreditCard Persisting(this CreditCard x, [Injected] DateTime now) => x with { ModifiedDate = now };

        public static (PersonCreditCard, ICreditCardCreator) Persisted(this CreditCard x) 
            => (new PersonCreditCard() { CreditCard = x, Person = x.ForContact }, ICreditCardCreator.CreatedCardHasBeenSaved(x.Creator, x));

        public static CreditCard Updating(this CreditCard x, [Injected] DateTime now) => x with { ModifiedDate = now };
    #endregion



    [MemberOrder(2), DisplayAsProperty]
        [Named("Card No.")]
        
        public static string ObfuscatedNumber(this CreditCard cc) {
            throw new NotImplementedException();

                //if (_ObfuscatedNumber == null && CardNumber != null && CardNumber.Length > 4) {
                //    _ObfuscatedNumber = CardNumber.Substring(CardNumber.Length - 4).PadLeft(CardNumber.Length, '*');
                //}
                //return _ObfuscatedNumber;
            }
 
      

        public static string Validate(this CreditCard cc, byte expMonth, short expYear) {
            if (expMonth == 0 || expYear == 0) {
                return null;
            }

            DateTime today = DateTime.Now.Date;
            DateTime expiryDate = new DateTime(expYear, expMonth, 1); //.EndOfMonth();

            if (expiryDate <= today) {
                return "Expiry date must be in the future";
            }
            return null;
        }

        public static string[] ChoicesCardType(this CreditCard cc) {
            return new[] {"Vista", "Distinguish", "SuperiorCard", "ColonialVoice"};
        }

        public static string ValidateCardNumber(this CreditCard cc,string cardNumber) {
            if (cardNumber != null && cardNumber.Length <= 4) {
                return "card number too short";
            }

            return null;
        }

        public static byte[] ChoicesExpMonth(this CreditCard cc)
            => Enumerable.Range(1, 12).Select(x => Convert.ToByte(x)).ToArray();


        public static short[] ChoicesExpYear()
            =>  new short[] {2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020};
    }
}