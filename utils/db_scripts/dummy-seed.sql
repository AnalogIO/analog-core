-- Seed the database with dummy data for local development.
-- Run from the root of the repository with:
--- cat utils/db_scripts/dummy-seed.sql | docker exec -i mssql /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "Your_password123" -d master

-- creates a verified user john@doe.com with password 1234.

SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @SchemaName NVARCHAR(128) = N'dbo';

    DECLARE @DummyUserEmail1 NVARCHAR(256) = N'john@doe.com';
    DECLARE @DummyUserEmail2 NVARCHAR(256) = N'dummy.seed.barista@analog.local';
    DECLARE @LegacyDummyUserEmail1 NVARCHAR(256) = N'dummy.seed.customer@analog.local';
    DECLARE @DummyUserPasswordHash NVARCHAR(256) = N'quXyHEeVCRjSW8GyPFObECr1/oiCrltPRCOUWYqiRx8=';

    DECLARE @ProgrammeShortName NVARCHAR(32) = N'DUMMY';
    DECLARE @ProgrammeFullName NVARCHAR(256) = N'Dummy Programme';
    DECLARE @DefaultProgrammeShortName NVARCHAR(32) = N'SWU';
    DECLARE @DefaultProgrammeFullName NVARCHAR(256) = N'Softwareudvikling';

    DECLARE @ProductName1 NVARCHAR(256) = N'Dummy Seed - 10 Coffee Clips';
    DECLARE @ProductName2 NVARCHAR(256) = N'Dummy Seed - 5 Tea Clips';

    DECLARE @MenuItemName1 NVARCHAR(256) = N'Dummy Seed - Cappuccino';
    DECLARE @MenuItemName2 NVARCHAR(256) = N'Dummy Seed - Latte';
    DECLARE @MenuItemName3 NVARCHAR(256) = N'Dummy Seed - Tea';

    DECLARE @ProgrammeId INT;
    DECLARE @UserId1 INT;
    DECLARE @UserId2 INT;
    DECLARE @ProductId1 INT;
    DECLARE @ProductId2 INT;
    DECLARE @MenuItemId1 INT;
    DECLARE @MenuItemId2 INT;
    DECLARE @MenuItemId3 INT;
    DECLARE @PurchaseId1 INT;
    DECLARE @PurchaseId2 INT;
    DECLARE @PurchaseId3 INT;
    DECLARE @PurchaseId4 INT;

    -- The register endpoint uses programme ID 1 as its backwards-compatible default.
    IF NOT EXISTS (SELECT 1 FROM dbo.Programmes WHERE Id = 1)
    BEGIN
        SET IDENTITY_INSERT dbo.Programmes ON;

        INSERT INTO dbo.Programmes (Id, ShortName, FullName, SortPriority)
        VALUES (1, @DefaultProgrammeShortName, @DefaultProgrammeFullName, 0);

        SET IDENTITY_INSERT dbo.Programmes OFF;
    END;

    -- Ensure a dedicated programme for dummy users exists.
    IF NOT EXISTS (SELECT 1 FROM dbo.Programmes WHERE ShortName = @ProgrammeShortName)
    BEGIN
        INSERT INTO dbo.Programmes (ShortName, FullName, SortPriority)
        VALUES (@ProgrammeShortName, @ProgrammeFullName, 999);
    END;

    SELECT @ProgrammeId = Id
    FROM dbo.Programmes
    WHERE ShortName = @ProgrammeShortName;

    -- Clean up previous dummy rows so this script is rerunnable.
    DELETE t
    FROM dbo.Tickets t
    INNER JOIN dbo.Users u ON u.Id = t.[Owner_Id]
    WHERE u.Email IN (@DummyUserEmail1, @DummyUserEmail2, @LegacyDummyUserEmail1);

    DELETE p
    FROM dbo.Purchases p
    INNER JOIN dbo.Users u ON u.Id = p.[PurchasedBy_Id]
    WHERE u.Email IN (@DummyUserEmail1, @DummyUserEmail2, @LegacyDummyUserEmail1);

    DELETE pug
    FROM dbo.ProductUserGroups pug
    INNER JOIN dbo.Products p ON p.Id = pug.ProductId
    WHERE p.Name IN (@ProductName1, @ProductName2);

    DELETE mip
    FROM dbo.MenuItemProducts mip
    INNER JOIN dbo.Products p ON p.Id = mip.ProductId
    WHERE p.Name IN (@ProductName1, @ProductName2);

    DELETE mip
    FROM dbo.MenuItemProducts mip
    INNER JOIN dbo.MenuItems mi ON mi.Id = mip.MenuItemId
    WHERE mi.Name IN (@MenuItemName1, @MenuItemName2, @MenuItemName3);

    DELETE FROM dbo.Products WHERE Name IN (@ProductName1, @ProductName2);
    DELETE FROM dbo.MenuItems WHERE Name IN (@MenuItemName1, @MenuItemName2, @MenuItemName3);
    DELETE FROM dbo.Users WHERE Email IN (@DummyUserEmail1, @DummyUserEmail2, @LegacyDummyUserEmail1);

    -- Insert dummy users.
    -- UserGroup values are ints: Customer=0, Barista=1, Manager=2, Board=3.
    INSERT INTO dbo.Users
        (Email, Name, Password, Salt, Experience, DateCreated, DateUpdated, IsVerified, PrivacyActivated, UserGroup, UserState, [Programme_Id])
    VALUES
        (@DummyUserEmail1, N'John Doe', @DummyUserPasswordHash, N'local-seed', 125, SYSUTCDATETIME(), SYSUTCDATETIME(), 1, 0, 0, N'Active', @ProgrammeId),
        (@DummyUserEmail2, N'Dummy Barista', N'not-a-real-password', N'local-seed', 430, SYSUTCDATETIME(), SYSUTCDATETIME(), 1, 0, 1, N'Active', @ProgrammeId);

    SELECT @UserId1 = Id FROM dbo.Users WHERE Email = @DummyUserEmail1;
    SELECT @UserId2 = Id FROM dbo.Users WHERE Email = @DummyUserEmail2;

    -- Insert dummy products.
    INSERT INTO dbo.Products
        (Price, NumberOfTickets, Name, Description, ExperienceWorth, Visible)
    VALUES
        (250, 10, @ProductName1, N'Dummy local seed product for grouped owned tickets', 10, 1),
        (150, 5, @ProductName2, N'Dummy local seed product for grouped owned tickets', 5, 1);

    SELECT @ProductId1 = Id FROM dbo.Products WHERE Name = @ProductName1;
    SELECT @ProductId2 = Id FROM dbo.Products WHERE Name = @ProductName2;

    INSERT INTO dbo.ProductUserGroups (ProductId, UserGroup)
    VALUES
        (@ProductId1, 0), -- Customer
        (@ProductId1, 2), -- Manager
        (@ProductId2, 0); -- Customer

    -- Insert dummy menu items.
    INSERT INTO dbo.MenuItems (Name, Active)
    VALUES
        (@MenuItemName1, 1),
        (@MenuItemName2, 1),
        (@MenuItemName3, 1);

    SELECT @MenuItemId1 = Id FROM dbo.MenuItems WHERE Name = @MenuItemName1;
    SELECT @MenuItemId2 = Id FROM dbo.MenuItems WHERE Name = @MenuItemName2;
    SELECT @MenuItemId3 = Id FROM dbo.MenuItems WHERE Name = @MenuItemName3;

    INSERT INTO dbo.MenuItemProducts (MenuItemId, ProductId)
    VALUES
        (@MenuItemId1, @ProductId1),
        (@MenuItemId2, @ProductId1),
        (@MenuItemId3, @ProductId2);

    -- Insert purchases and tickets for dummy customer.
    INSERT INTO dbo.Purchases
        (ProductName, ProductId, Price, NumberOfTickets, DateCreated, OrderId, ExternalTransactionId, Status, Type, [PurchasedBy_Id])
    VALUES
        (@ProductName1, @ProductId1, 250, 10, SYSUTCDATETIME(), CONVERT(NVARCHAR(36), NEWID()), NULL, N'Completed', N'MobilePayV2', @UserId1),
        (@ProductName2, @ProductId2, 0, 5, SYSUTCDATETIME(), CONVERT(NVARCHAR(36), NEWID()), NULL, N'Completed', N'Free', @UserId1),
        (@ProductName1, @ProductId1, 250, 10, SYSUTCDATETIME(), CONVERT(NVARCHAR(36), NEWID()), NULL, N'Completed', N'MobilePayV2', @UserId2),
        (@ProductName2, @ProductId2, 0, 5, SYSUTCDATETIME(), CONVERT(NVARCHAR(36), NEWID()), NULL, N'Completed', N'Free', @UserId2);

    SELECT TOP(1) @PurchaseId1 = Id
    FROM dbo.Purchases
    WHERE [PurchasedBy_Id] = @UserId1 AND ProductId = @ProductId1
    ORDER BY Id DESC;

    SELECT TOP(1) @PurchaseId2 = Id
    FROM dbo.Purchases
    WHERE [PurchasedBy_Id] = @UserId1 AND ProductId = @ProductId2
    ORDER BY Id DESC;

    SELECT TOP(1) @PurchaseId3 = Id
    FROM dbo.Purchases
    WHERE [PurchasedBy_Id] = @UserId2 AND ProductId = @ProductId1
    ORDER BY Id DESC;

    SELECT TOP(1) @PurchaseId4 = Id
    FROM dbo.Purchases
    WHERE [PurchasedBy_Id] = @UserId2 AND ProductId = @ProductId2
    ORDER BY Id DESC;

    INSERT INTO dbo.Tickets
        (DateCreated, DateUsed, ProductId, IsUsed, Status, [Owner_Id], [Purchase_Id], UsedOnMenuItemId)
    VALUES
        (SYSUTCDATETIME(), NULL, @ProductId1, 0, 0, @UserId1, @PurchaseId1, NULL),
        (SYSUTCDATETIME(), NULL, @ProductId1, 0, 0, @UserId1, @PurchaseId1, NULL),
        (SYSUTCDATETIME(), NULL, @ProductId1, 0, 0, @UserId1, @PurchaseId1, NULL),
        (SYSUTCDATETIME(), SYSUTCDATETIME(), @ProductId1, 1, 1, @UserId1, @PurchaseId1, @MenuItemId1),
        (SYSUTCDATETIME(), NULL, @ProductId2, 0, 0, @UserId1, @PurchaseId2, NULL),
        (SYSUTCDATETIME(), NULL, @ProductId1, 0, 0, @UserId2, @PurchaseId3, NULL),
        (SYSUTCDATETIME(), SYSUTCDATETIME(), @ProductId1, 1, 1, @UserId2, @PurchaseId3, @MenuItemId2),
        (SYSUTCDATETIME(), NULL, @ProductId2, 0, 0, @UserId2, @PurchaseId4, NULL),
        (SYSUTCDATETIME(), SYSUTCDATETIME(), @ProductId2, 1, 1, @UserId2, @PurchaseId4, @MenuItemId3);

    COMMIT TRANSACTION;

    SELECT
        @UserId1 AS DummyCustomerUserId,
        @UserId2 AS DummyBaristaUserId,
        @ProductId1 AS DummyProductCoffeeId,
        @ProductId2 AS DummyProductTeaId,
        @PurchaseId1 AS DummyPurchaseCoffeeId,
        @PurchaseId2 AS DummyPurchaseTeaId,
        @PurchaseId3 AS DummyBaristaPurchaseCoffeeId,
        @PurchaseId4 AS DummyBaristaPurchaseTeaId;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    THROW;
END CATCH;