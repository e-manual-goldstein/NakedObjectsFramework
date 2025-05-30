using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel.Mapping
{
    public class SalesOrderHeaderMap : EntityTypeConfiguration<SalesOrderHeader>
    {
        public SalesOrderHeaderMap()
        {
            // Primary Key
            HasKey(t => t.SalesOrderID);

            //Ignores
            Ignore(t => t.AddItemsFromCart);

            // Properties
            Property(t => t.SalesOrderNumber)
                .IsRequired()
                .HasMaxLength(25).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            Property(t => t.PurchaseOrderNumber)
                .HasMaxLength(25);

            Property(t => t.AccountNumber)
                .HasMaxLength(15);

            Property(t => t.CreditCardApprovalCode)
                .HasMaxLength(15);

            Property(t => t.Comment)
                .HasMaxLength(128);

            // Table & Column Mappings
            ToTable("SalesOrderHeader", "Sales");
            Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            Property(t => t.RevisionNumber).HasColumnName("RevisionNumber");
            Property(t => t.OrderDate).HasColumnName("OrderDate");
            Property(t => t.DueDate).HasColumnName("DueDate");
            Property(t => t.ShipDate).HasColumnName("ShipDate");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.OnlineOrder).HasColumnName("OnlineOrderFlag");
            Property(t => t.SalesOrderNumber).HasColumnName("SalesOrderNumber");
            Property(t => t.PurchaseOrderNumber).HasColumnName("PurchaseOrderNumber");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            Property(t => t.BillingAddressID).HasColumnName("BillToAddressID");
            Property(t => t.ShippingAddressID).HasColumnName("ShipToAddressID");
            Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            Property(t => t.CreditCardApprovalCode).HasColumnName("CreditCardApprovalCode");
            Property(t => t.CurrencyRateID).HasColumnName("CurrencyRateID");
            Property(t => t.SubTotal).HasColumnName("SubTotal");
            Property(t => t.TaxAmt).HasColumnName("TaxAmt");
            Property(t => t.Freight).HasColumnName("Freight");
            Property(t => t.TotalDue).HasColumnName("TotalDue").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed); ;
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.BillingAddress).WithMany().HasForeignKey(t => t.BillingAddressID);
            HasRequired(t => t.ShippingAddress).WithMany().HasForeignKey(t => t.ShippingAddressID);
            HasRequired(t => t.ShipMethod).WithMany().HasForeignKey(t => t.ShipMethodID);
            HasOptional(t => t.CreditCard).WithMany().HasForeignKey(t => t.CreditCardID);
            HasOptional(t => t.CurrencyRate).WithMany().HasForeignKey(t => t.CurrencyRateID);
            HasRequired(t => t.Customer).WithMany().HasForeignKey(t => t.CustomerID);
            HasOptional(t => t.SalesPerson).WithMany().HasForeignKey(t => t.SalesPersonID);
            HasOptional(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesOrderHeader> builder)
        {
            builder.HasKey(t => t.SalesOrderID);

            //Ignores
            builder.Ignore(t => t.AddItemsFromCart);
            //Ignore(t => t.Status); //TODO is it necessary to ignore this?


            // Properties
            builder.Property(t => t.SalesOrderNumber)
                .IsRequired()
                .HasMaxLength(25).ValueGeneratedOnAddOrUpdate();

            builder.Property(t => t.PurchaseOrderNumber)
                .HasMaxLength(25);

            builder.Property(t => t.AccountNumber)
                .HasMaxLength(15);

            builder.Property(t => t.CreditCardApprovalCode)
                .HasMaxLength(15);

            builder.Property(t => t.Comment)
                .HasMaxLength(128);

            // Table & Column Mappings
            builder.ToTable("SalesOrderHeader", "Sales");
            builder.Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            builder.Property(t => t.RevisionNumber).HasColumnName("RevisionNumber");
            builder.Property(t => t.OrderDate).HasColumnName("OrderDate");
            builder.Property(t => t.DueDate).HasColumnName("DueDate");
            builder.Property(t => t.ShipDate).HasColumnName("ShipDate");
            //builder.Property(t => t.StatusByte).HasColumnName("Status");
            builder.Property(t => t.OnlineOrder).HasColumnName("OnlineOrderFlag");
            builder.Property(t => t.SalesOrderNumber).HasColumnName("SalesOrderNumber");
            builder.Property(t => t.PurchaseOrderNumber).HasColumnName("PurchaseOrderNumber");
            builder.Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            builder.Property(t => t.CustomerID).HasColumnName("CustomerID");
            builder.Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            builder.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.BillingAddressID).HasColumnName("BillToAddressID");
            builder.Property(t => t.ShippingAddressID).HasColumnName("ShipToAddressID");
            builder.Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            builder.Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            builder.Property(t => t.CreditCardApprovalCode).HasColumnName("CreditCardApprovalCode");
            builder.Property(t => t.CurrencyRateID).HasColumnName("CurrencyRateID");
            builder.Property(t => t.SubTotal).HasColumnName("SubTotal");
            builder.Property(t => t.TaxAmt).HasColumnName("TaxAmt");
            builder.Property(t => t.Freight).HasColumnName("Freight");
            builder.Property(t => t.TotalDue).HasColumnName("TotalDue").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.Comment).HasColumnName("Comment");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.BillingAddress).WithMany().HasForeignKey(t => t.BillingAddressID);
            builder.HasOne(t => t.ShippingAddress).WithMany().HasForeignKey(t => t.ShippingAddressID);
            builder.HasOne(t => t.ShipMethod).WithMany().HasForeignKey(t => t.ShipMethodID);
            builder.HasOne(t => t.CreditCard).WithMany().HasForeignKey(t => t.CreditCardID);
            builder.HasOne(t => t.CurrencyRate).WithMany().HasForeignKey(t => t.CurrencyRateID);
            builder.HasOne(t => t.Customer).WithMany().HasForeignKey(t => t.CustomerID);
            builder.HasOne(t => t.SalesPerson).WithMany().HasForeignKey(t => t.SalesPersonID);
            builder.HasOne(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
        }
    }
}
