using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class PurchaseDataCleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set the Type Column on the purchases table
            migrationBuilder.Sql("UPDATE dbo.Purchases SET Type = 'Voucher' WHERE TransactionId like 'VOUCHER%'");
            migrationBuilder.Sql("UPDATE dbo.Purchases SET Type = 'Free' Where TransactionId = '00000000-0000-0000-0000-000000000000'");
            migrationBuilder.Sql("UPDATE dbo.Purchases SET Type = 'MobilePayV1' Where (len(TransactionId) < 36 and TransactionId not like 'VOUCHER%' and not OrderId = 'Analog')");
            migrationBuilder.Sql("UPDATE dbo.Purchases SET Type = 'MobilePayV1' Where TransactionId is null");
            migrationBuilder.Sql("UPDATE dbo.Purchases SET Type = 'MobilePayV2' Where (len(TransactionId) = 36 and not TransactionId = '00000000-0000-0000-0000-000000000000') ");
            migrationBuilder.Sql("UPDATE dbo.Purchases SET Type = 'PointOfSale' Where OrderId like 'Analog'");

            // Update vouchers with references to the purchase issued by its redemption
            migrationBuilder.Sql("update Vouchers set PurchaseId = Purchases.Id from Purchases where Purchases.OrderId = Vouchers.Code");

            // Create new vouchers for purchases where the same voucher have been used to issue multiple purchase
            migrationBuilder.Sql("insert into Vouchers (Code, DateCreated, DateUsed, Description, Requester, Product_Id, User_Id, PurchaseId) select Purchases.OrderId + '-' + cast(Purchases.Id as varchar), Purchases.DateCreated, Purchases.DateCreated, 'Creation of extra vouchers for purchases without references', 'AnalogIO', Purchases.ProductId, Purchases.PurchasedBy_Id, Id from Purchases where Type = 'Voucher' and Id not in (select PurchaseId from Vouchers where PurchaseId is not null)");
            // Insert data to the new Pos table
            migrationBuilder.Sql("insert into PosPurchases (PurchaseId, BaristaInitials) select Id, TransactionId from Purchases where OrderId like 'Analog'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
