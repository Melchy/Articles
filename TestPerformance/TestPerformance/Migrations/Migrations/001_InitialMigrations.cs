using FluentMigrator;
using Migrations;
using Npgsql;
using System;
using System.Data;

namespace Migrations;

[Migration(1)]
public class InitialMigrations : Migration
{
    public override void Up()
    {
        Create.Table("Users")
           .WithColumn("Id").AsInt64().PrimaryKey()
           .WithColumn("Name").AsString().Nullable()
           .WithColumn("Surname").AsString().Nullable()
           .WithColumn("Email").AsString().Nullable()
           .WithColumn("Password").AsString().Nullable()
           .WithColumn("BirthDate").AsDateTimeOffset().Nullable()
            .WithColumn("CartId").AsInt64().Nullable();
        
        Create.Table("Carts")
           .WithColumn("Id").AsInt64().PrimaryKey()
           .WithColumn("Amount").AsInt32().Nullable();

        Create.Table("CartItems")
           .WithColumn("Id").AsInt64().PrimaryKey()
           .WithColumn("ItemsId").AsInt64().Nullable()
           .WithColumn("CartsId").AsInt64().Nullable();
        
        Create.Table("Items")
           .WithColumn("Id").AsInt64().PrimaryKey()
           .WithColumn("Name").AsString().Nullable()
           .WithColumn("Description").AsString().Nullable()
           .WithColumn("ShortDescription").AsString().Nullable()
           .WithColumn("AmountInWereHouse").AsInt64().Nullable();

        Create.ForeignKey().FromTable("Users").ForeignColumn("CartId").ToTable("Carts").PrimaryColumn("Id");
        Create.ForeignKey().FromTable("CartItems").ForeignColumn("ItemsId").ToTable("Items").PrimaryColumn("Id");
        Create.ForeignKey().FromTable("CartItems").ForeignColumn("CartsId").ToTable("Carts").PrimaryColumn("Id");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}
