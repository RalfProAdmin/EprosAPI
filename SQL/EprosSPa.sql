USE [EprosCareersDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_RegisterUserWithProfile]    Script Date: 15-04-2026 19:35:00 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_RegisterUserWithProfile]
GO
/****** Object:  StoredProcedure [dbo].[sp_RegisterUserWithProfile]    Script Date: 15-04-2026 19:35:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_RegisterUserWithProfile]
    @FullName NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(MAX),
    @RoleId INT,
    @CurrentRole NVARCHAR(100),
    @TotalExperience DECIMAL(4,2),
    @Summary NVARCHAR(MAX),
    @KeySkills NVARCHAR(MAX),
    @ResumeUrl NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- 1. Insert into Users
        INSERT INTO [dbo].[Users] (FullName, Email, PasswordHash, RoleId, CreatedAt, UpdatedAt, IsActive)
        VALUES (@FullName, @Email, @PasswordHash, @RoleId, GETDATE(), GETDATE(), 1);

        -- Get the newly created UserId
        DECLARE @NewUserId INT = SCOPE_IDENTITY();

        -- 2. Insert into UserProfiles
        INSERT INTO [dbo].[UserProfiles] (UserID, CurrentRole, TotalExperience, Summary, KeySkills, ResumeUrl)
        VALUES (@NewUserId, @CurrentRole, @TotalExperience, @Summary, @KeySkills, @ResumeUrl);

        COMMIT TRANSACTION;
        SELECT @NewUserId AS NewUserId; -- Return the ID to the API
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
