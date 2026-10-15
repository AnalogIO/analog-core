-- Mark a user as verified by email.
-- Run from the root of the repository with:
--- cat utils/db_scripts/verify-user.sql | docker exec -i mssql /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "Your_password123" -v UserEmail="john@doe.com" -d master

SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;

DECLARE @UserEmail NVARCHAR(256) = N'$(UserEmail)';

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.Users
    SET IsVerified = 1,
        DateUpdated = SYSUTCDATETIME()
    WHERE Email = @UserEmail;

    IF @@ROWCOUNT = 0
    BEGIN
        THROW 50000, 'No user found with the supplied email address.', 1;
    END;

    COMMIT TRANSACTION;

    SELECT Email, IsVerified
    FROM dbo.Users
    WHERE Email = @UserEmail;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    THROW;
END CATCH;
