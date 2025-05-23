﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace NakedFunctions.Rest.Test.Data;

public static class EFCoreConstants {
    public static string AppveyorServer => @"(local)\SQL2017";
    public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
    public static string Server => LocalServer;
#endif

    public static readonly string CsMenu = @$"Data Source={Server};Initial Catalog={"MenuRestTests"};Integrated Security=True;Encrypt=False;";
    public static readonly string CsObject = @$"Data Source={Server};Initial Catalog={"ObjectRestTests"};Integrated Security=True;Encrypt=False;";
}

public class BlankTriggerAddingConvention : IModelFinalizingConvention {
    public virtual void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context) {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes()) {
            var table = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
            if (table != null
                && entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(table.Value) == null)) {
                entityType.Builder.HasTrigger(table.Value.Name + "_Trigger");
            }

            foreach (var fragment in entityType.GetMappingFragments(StoreObjectType.Table)) {
                if (entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(fragment.StoreObject) == null)) {
                    entityType.Builder.HasTrigger(fragment.StoreObject.Name + "_Trigger");
                }
            }
        }
    }
}

public abstract class EFCoreTestDbContext : DbContext {
    private readonly string cs;

    protected EFCoreTestDbContext(string cs) => this.cs = cs;

    public DbSet<SimpleRecord> SimpleRecords { get; set; }
    public DbSet<DateRecord> DateRecords { get; set; }
    public DbSet<EnumRecord> EnumRecords { get; set; }
    public DbSet<GuidRecord> GuidRecords { get; set; }
    public DbSet<ReferenceRecord> ReferenceRecords { get; set; }
    public DbSet<DisplayAsPropertyRecord> DisplayAsPropertyRecords { get; set; }
    public DbSet<UpdatedRecord> UpdatedRecords { get; set; }
    public DbSet<CollectionRecord> CollectionRecords { get; set; }
    public DbSet<OrderedRecord> OrderedRecords { get; set; }
    public DbSet<EditRecord> EditRecords { get; set; }
    public DbSet<DeleteRecord> DeleteRecords { get; set; }
    public DbSet<BoundedRecord> BoundedRecords { get; set; }
    public DbSet<ByteArrayRecord> ByteArrayRecords { get; set; }
    public DbSet<MaskRecord> MaskRecords { get; set; }
    public DbSet<HiddenRecord> HiddenRecords { get; set; }
    public DbSet<AlternateKeyRecord> AlternateKeyRecords { get; set; }
    public DbSet<UrlLinkRecord> UrlLinkRecords { get; set; }

    public DbSet<NToNCollectionRecord1> NToNCollectionRecord1 { get; set; }

    public DbSet<NToNCollectionRecord2> NToNCollectionRecord2 { get; set; }

    public DbSet<MultilineRecord> MultilineRecords { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(cs);
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var fred = new SimpleRecord { Id = 1, Name = "Fred" };

        modelBuilder.Entity<SimpleRecord>().HasData(fred);
        modelBuilder.Entity<SimpleRecord>().HasData(new SimpleRecord { Id = 2, Name = "Bill" });
        modelBuilder.Entity<SimpleRecord>().HasData(new SimpleRecord { Id = 3, Name = "Jack" });
        modelBuilder.Entity<SimpleRecord>().HasData(new SimpleRecord { Id = 4, Name = "hide it" });

        var ur = new UpdatedRecord { Id = 1, Name = "" };
        modelBuilder.Entity<UpdatedRecord>().HasData(ur);

        var dr = new DateRecord { Id = 1, StartDate = DateTime.Now, EndDate = null };

        modelBuilder.Entity<DateRecord>().HasData(dr);
        modelBuilder.Entity<EnumRecord>().HasData(new EnumRecord { Id = 1 });

        modelBuilder.Entity<ReferenceRecord>().HasData(new { Id = 1, UpdatedRecordId = 1, DateRecordId = 1 });

        modelBuilder.Entity<CollectionRecord>().HasData(new CollectionRecord { Id = 1 });

        modelBuilder.Entity<GuidRecord>().HasData(new GuidRecord { Id = 1 });

        modelBuilder.Entity<DisplayAsPropertyRecord>().HasData(new DisplayAsPropertyRecord { Id = 1 });

        modelBuilder.Entity<OrderedRecord>().HasData(new OrderedRecord { Id = 1 });

        modelBuilder.Entity<EditRecord>().HasData(new { Id = 1, Name = "Jane", SimpleRecordId = 1, NotMatched = "no" });

        modelBuilder.Entity<DeleteRecord>().HasData(new DeleteRecord { Id = 1 });
        modelBuilder.Entity<DeleteRecord>().HasData(new DeleteRecord { Id = 2 });

        modelBuilder.Entity<BoundedRecord>().HasData(new BoundedRecord { Id = 1, Name = "One" });
        modelBuilder.Entity<BoundedRecord>().HasData(new BoundedRecord { Id = 2, Name = "Two" });

        modelBuilder.Entity<ByteArrayRecord>().HasData(new ByteArrayRecord { Id = 1 });

        modelBuilder.Entity<MaskRecord>().Ignore(m => m.MaskRecordProperty);
        modelBuilder.Entity<MaskRecord>().HasData(new MaskRecord { Id = 1, Name = "Title" });
        modelBuilder.Entity<HiddenRecord>().HasData(new MaskRecord { Id = 1, Name = "Title" });

        modelBuilder.Entity<MultilineRecord>().HasData(new MultilineRecord { Id = 1, Name = "Title" });

        modelBuilder.Entity<AlternateKeyRecord>().HasAlternateKey(k => k.Name);
        modelBuilder.Entity<AlternateKeyRecord>().HasData(new AlternateKeyRecord { Id = 1, Name = "AK1" });

        modelBuilder.Entity<UrlLinkRecord>().HasData(new UrlLinkRecord() { Id = 1, Link1 = "Link1Name", Link2 = "Link2Name", Link3 = "Link3Name", Link4 = "Link4Name" });
    }
}

public class EFCoreMenuDbContext : EFCoreTestDbContext {
    public EFCoreMenuDbContext() : base(Constants.CsMenu) { }
    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();
}

public class EFCoreServiceDbContext : EFCoreTestDbContext {
    public EFCoreServiceDbContext() : base(Constants.CsService) { }
    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();
}



public class EFCoreObjectDbContext : EFCoreTestDbContext {
    public EFCoreObjectDbContext() : base(Constants.CsObject) { }
    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();
}

public class EFCoreNullabilityDbContext : EFCoreTestDbContext {
    public EFCoreNullabilityDbContext() : base(Constants.CsNull) { }
    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();
}