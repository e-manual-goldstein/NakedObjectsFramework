// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

using System.Data.Entity;

namespace SimpleDatabase;

public class SimpleDatabaseDbContext : DbContext {
    public SimpleDatabaseDbContext(string name) : base(name) { }
    public SimpleDatabaseDbContext() { }

    //Add DbSet properties for root objects, thus:
    public DbSet<Person> Persons { get; set; }
    public DbSet<Fruit> Fruits { get; set; }
    public DbSet<Food> Foods { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder) { }
}