// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.Persistor.Entity.Test.AdventureWorksCodeOnly;

[Table("Production.TransactionHistoryArchive")]
public class TransactionHistoryArchive {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TransactionID { get; set; }

    public int ProductID { get; set; }

    public int ReferenceOrderID { get; set; }

    public int ReferenceOrderLineID { get; set; }

    public DateTime TransactionDate { get; set; }

    [Required]
    [StringLength(1)]
    public string TransactionType { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "money")]
    public decimal ActualCost { get; set; }

    public DateTime ModifiedDate { get; set; }
}