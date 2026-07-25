-- STORED PROCEDURES:
CREATE PROCEDURE TravelCo_sp_Country_GetAll
AS
BEGIN
    SELECT Code, Name, Capital, Region, Population, Area, Flag FROM TravelCo_Countries;
END
GO

--------------
CREATE PROCEDURE TravelCo_sp_Country_GetByCode
    @Code NVARCHAR(3)
AS
BEGIN
    SELECT Code, Name, Capital, Region, Population, Area, Flag
    FROM TravelCo_Countries
    WHERE Code = @Code;
END
GO

--------------
CREATE PROCEDURE TravelCo_sp_User_Create
    @Username NVARCHAR(50),
    @Email NVARCHAR(256),
    @PasswordHash NVARCHAR(256),
    @PasswordSalt NVARCHAR(256)
AS
BEGIN
    INSERT INTO TravelCo_Users (Username, Email, PasswordHash, PasswordSalt)
    VALUES (@Username, @Email, @PasswordHash, @PasswordSalt);

    SELECT SCOPE_IDENTITY();   -- returns the Id that was just auto-generated
END
GO

--------------
CREATE PROCEDURE TravelCo_sp_User_GetByEmail
    @Email NVARCHAR(256)
AS
BEGIN
    SELECT Id, Username, Email, PasswordHash, PasswordSalt, Role, IsLocked, CanShare, CreatedAt
    FROM TravelCo_Users
    WHERE Email = @Email;
END
GO

--------------
--UserLists table only stores CountryCode, but the client wants full country details. 
--A JOIN stitches the two tables together on the matching code
CREATE PROCEDURE TravelCo_sp_List_Get
    @UserId INT
AS
BEGIN
    SELECT c.Code, c.Name, c.Capital, c.Region, c.Population, c.Area, c.Flag, l.ListType
    FROM TravelCo_UserLists l
    INNER JOIN TravelCo_Countries c ON l.CountryCode = c.Code
    WHERE l.UserId = @UserId;
END
GO

--------------
--Add or move a country to a list
CREATE PROCEDURE TravelCo_sp_List_Add
    @UserId INT,
    @CountryCode NVARCHAR(3),
    @ListType NVARCHAR(20)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM TravelCo_UserLists WHERE UserId = @UserId AND CountryCode = @CountryCode)
        UPDATE TravelCo_UserLists SET ListType = @ListType
        WHERE UserId = @UserId AND CountryCode = @CountryCode;
    ELSE
        INSERT INTO TravelCo_UserLists (UserId, CountryCode, ListType)
        VALUES (@UserId, @CountryCode, @ListType);
END
GO

--------------
--Remove a country from a list
CREATE PROCEDURE TravelCo_sp_List_Remove
    @UserId INT,
    @CountryCode NVARCHAR(3)
AS
BEGIN
    DELETE FROM TravelCo_UserLists
    WHERE UserId = @UserId AND CountryCode = @CountryCode;
END
GO

--------------
--Move a country from one list to another
CREATE PROCEDURE TravelCo_sp_List_Move
    @UserId INT,
    @CountryCode NVARCHAR(3),
    @ToType NVARCHAR(20)
AS
BEGIN
    UPDATE TravelCo_UserLists SET ListType = @ToType
    WHERE UserId = @UserId AND CountryCode = @CountryCode;
END
GO

--------------
--callers can skip the parameter
CREATE PROCEDURE TravelCo_sp_Share_GetAll
    @CountryCode NVARCHAR(3) = NULL
AS
BEGIN
    SELECT s.Id, s.UserId, s.CountryCode, c.Name AS CountryName,
           s.Type, s.Title, s.Body, u.Username AS Author, s.CreatedAt
    FROM TravelCo_Shares s
    INNER JOIN TravelCo_Users u     ON s.UserId = u.Id
    INNER JOIN TravelCo_Countries c ON s.CountryCode = c.Code
    WHERE (@CountryCode IS NULL OR s.CountryCode = @CountryCode)
    ORDER BY s.CreatedAt DESC, s.Id DESC;
END
GO

--------------
-- Create a new share
CREATE PROCEDURE TravelCo_sp_Share_Create
    @UserId INT, @CountryCode NVARCHAR(3), @Type NVARCHAR(20),
    @Title NVARCHAR(150), @Body NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO TravelCo_Shares (UserId, CountryCode, Type, Title, Body)
    VALUES (@UserId, @CountryCode, @Type, @Title, @Body);
    SELECT SCOPE_IDENTITY();
END
GO

--------------
-- update a share, but only if the user owns it
CREATE PROCEDURE TravelCo_sp_Share_Update
    @Id INT, @UserId INT, @Type NVARCHAR(20),
    @Title NVARCHAR(150), @Body NVARCHAR(MAX)
AS
BEGIN
    UPDATE TravelCo_Shares
    SET Type = @Type, Title = @Title, Body = @Body
    WHERE Id = @Id AND UserId = @UserId;
END
GO

--------------
--Delete share, but only if the user owns it
CREATE PROCEDURE TravelCo_sp_Share_Delete
    @Id INT, @UserId INT
AS
BEGIN
    DELETE FROM TravelCo_Shares WHERE Id = @Id AND UserId = @UserId;
END
GO

--------------
--List all users
CREATE PROCEDURE TravelCo_sp_Admin_Users
AS
BEGIN
    SELECT Id, Username, Email, Role, IsLocked, CanShare, CreatedAt
    FROM TravelCo_Users
    ORDER BY Id;
END
GO

--------------
--Set a user's IsLocked and CanShare flags
CREATE PROCEDURE TravelCo_sp_Admin_SetUserFlags
    @Id INT, @IsLocked BIT, @CanShare BIT
AS
BEGIN
    UPDATE TravelCo_Users
    SET IsLocked = @IsLocked, CanShare = @CanShare
    WHERE Id = @Id;
END
GO

--------------
--Usage stats (to display to admin users on the dashboard)
CREATE PROCEDURE TravelCo_sp_Admin_Stats
AS
BEGIN
    SELECT
        (SELECT COUNT(*) FROM TravelCo_LoginEvents
            WHERE CAST(LoggedAt AS DATE) = CAST(GETUTCDATE() AS DATE)) AS DailyLogins,
        (SELECT COUNT(*) FROM TravelCo_Countries)  AS CountriesImported,
        (SELECT COUNT(*) FROM TravelCo_UserLists)  AS CountriesSaved,
        (SELECT COUNT(*) FROM TravelCo_Shares)     AS SharesCreated;
END
GO

--------------
--Record a login
CREATE PROCEDURE TravelCo_sp_LoginEvent_Add
    @UserId INT
AS
BEGIN
    INSERT INTO TravelCo_LoginEvents (UserId) VALUES (@UserId);
END