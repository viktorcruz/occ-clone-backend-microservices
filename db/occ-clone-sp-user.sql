

USE [OCC_Clone]
GO

IF OBJECT_ID('[dbo].[Usp_Users_Add]', 'P') IS NOT NULL
BEGIN 
	DROP PROCEDURE [dbo].[Usp_Users_Add]
END
GO
IF OBJECT_ID('[dbo].[Usp_Users_Get]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_Users_Get]
END
GO
IF OBJECT_ID('[dbo].[Usp_UsersByCredentials_Get]', 'P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_UsersByCredentials_Get]
END
GO
IF OBJECT_ID('[dbo].[Usp_UsersByEmail_Get]', 'P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_UsersByEmail_Get]
END
GO
IF OBJECT_ID('[dbo].[Usp_Users_GetAll]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_Users_GetAll]
END
GO
IF OBJECT_ID('[dbo].[Ups_Users_Update]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Ups_Users_Update]
END
GO
IF OBJECT_ID('[dbo].[Usp_Users_Delete]', 'P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_Users_Delete]
END
GO
IF OBJECT_ID('[dbo].[Ups_UserPassword_Update]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Ups_UserPassword_Update]
END
GO
IF OBJECT_ID('[dbo].[Usp_Users_Activate]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_Users_Activate]
END
GO

/** INSERT **/
CREATE OR ALTER PROCEDURE
	Usp_Users_Add
		@IdRole INT,
		@FirstName NVARCHAR(20),
		@LastName NVARCHAR(20),
		@Email NVARCHAR(50),
		@PasswordHash NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Result AS TABLE
	(
		ResultStatus BIT,
		ResultMessage VARCHAR(100),
		OperationType VARCHAR(20),
		AffectedRecordId INT,
		OperationDateTime DATETIME
	)
	
	BEGIN TRY
		BEGIN TRANSACTION 

		IF EXISTS(SELECT 1 FROM [dbo].[Users] WHERE Email = @Email)
		BEGIN 
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			SELECT 0, 'Error: User already exists', 'NONE', NULL, GETDATE()

            ROLLBACK TRANSACTION;
            RETURN;
		END
		ELSE BEGIN
			INSERT INTO [dbo].[Users](IdRole, FirstName, LastName, Email, PasswordHash, CreationDate)
			VALUES(@IdRole, @FirstName, @LastName, @Email, @PasswordHash, GETDATE())

			DECLARE @LastId INT = (SELECT SCOPE_IDENTITY())

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			SELECT 1, 'Data has been sucessfully inserted', 'INSERT', @LastId, GETDATE()

			COMMIT TRANSACTION 
		END

	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION 
		END

		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, ERROR_MESSAGE(), 'ERROR', NULL, GETDATE())

	END CATCH
	
	SET NOCOUNT OFF;
	SELECT * FROM @Result
END
GO


/** GET **/
CREATE OR ALTER PROCEDURE
	Usp_Users_Get
		@IdUser INT
AS
BEGIN
	SET NOCOUNT ON;
		DECLARE @Result AS TABLE
		(
			ResultStatus BIT,
			ResultMessage VARCHAR(100),
			OperationType VARCHAR(20),
			AffectedRecordId INT,
			OperationDateTime DATETIME,
			IdUser INT,
			IdRole INT,
			FirstName VARCHAR(20),
			LastName VARCHAR(20),
			Email VARCHAR(50),
			PasswordHash VARCHAR(100),
			CreationDate DATETIME,
			IsActive INT,
			IsRegistrationConfirmed INT
		)

		BEGIN TRY
			IF EXISTS( SELECT 1 FROM Users (nolock) WHERE IdUser = @IdUser )
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, FirstName, LastName, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed)
				SELECT 1, 'User found', 'GET', 0, GETDATE(), IdUser, IdRole, FirstName, LastName, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed
				FROM Users (nolock)  
				WHERE IdUser = @IdUser

			END
			ELSE
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, FirstName, LastName, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed)
				VALUES(0, 'Error: User not found', 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
			END
		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, FirstName, LastName, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed)
			VALUES(0, @ErrorMessage, 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL,  NULL, NULL)
		END CATCH
	
	SET NOCOUNT OFF;

	SELECT * FROM @Result
END
GO

/** GET **/
CREATE OR ALTER PROCEDURE
	Usp_UsersByCredentials_Get
		@Email NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
		DECLARE @Result AS TABLE
		(
			ResultStatus BIT,
			ResultMessage VARCHAR(100),
			OperationType VARCHAR(20),
			AffectedRecordId INT,
			OperationDateTime DATETIME,
			IdUser INT,
			IdRole INT,
			Email VARCHAR(50),
			PasswordHash VARCHAR(100),
			CreationDate DATETIME,
			IsActive INT,
			IsRegistrationConfirmed INT
		)

		BEGIN TRY
			IF EXISTS( SELECT 1 FROM Users (nolock) WHERE Email = @Email)
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed)
				SELECT 1, 'User found', 'GET', 0, GETDATE(), IdUser, IdRole, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed
				FROM Users (nolock)  
				WHERE Email = @Email 

			END
			ELSE
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed)
				VALUES(0, 'Error: User not found', 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL)
			END
		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed)
			VALUES(0, @ErrorMessage, 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL)
		END CATCH
	
	SET NOCOUNT OFF;

	SELECT * FROM @Result
END
GO

/** GET **/
CREATE OR ALTER PROCEDURE
	Usp_UsersByEmail_Get
		@Email NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
		DECLARE @Result AS TABLE
		(
			ResultStatus BIT,
			ResultMessage VARCHAR(100),
			OperationType VARCHAR(20),
			AffectedRecordId INT,
			OperationDateTime DATETIME,
			IdUser INT,
			Email VARCHAR(50),
			PasswordHash VARCHAR(100),
			CreationDate DATETIME
		)

		BEGIN TRY
			IF EXISTS( SELECT 1 FROM Users (nolock) WHERE Email = @Email)
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, Email, PasswordHash, CreationDate)
				SELECT 1, 'User found', 'GET', 0, GETDATE(), IdUser, Email, PasswordHash, CreationDate
				FROM Users (nolock)  
				WHERE Email = @Email 

			END
			ELSE
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, Email, PasswordHash, CreationDate)
				VALUES(0, 'Error: User not found', 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL)
			END
		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, Email, PasswordHash, CreationDate)
			VALUES(0, @ErrorMessage, 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL)
		END CATCH
	
	SET NOCOUNT OFF;

	SELECT * FROM @Result
END
GO

/** CONFIRM REGISTER **/
CREATE OR ALTER PROCEDURE
	Usp_Users_ConfirmRegister
		@IdUser INT,
		@Email NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ErrorMessage NVARCHAR(4000);
	DECLARE @Result AS TABLE
	(
		ResultStatus INT, 
		ResultMessage NVARCHAR(100),
		OperationType NVARCHAR(20),
		AffectedRecordId INT,
		OperationDateTime DATETIME
	);

	BEGIN TRY
		BEGIN TRANSACTION TUP
			IF NOT EXISTS(SELECT 1 FROM Users WHERE IdUser = @IdUser AND Email = @Email)
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
				VALUES(0, 'Error: User not found', 'NONE', NULL, GETDATE())

				ROLLBACK TRANSACTION TUP
				RETURN;
			END
			ELSE BEGIN
				UPDATE Users
					SET IsActive = 1,
						IsRegistrationConfirmed = 1,
						RegistrationConfirmedAt = GETDATE()
				WHERE IdUser = @IdUser AND Email = @Email

				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
				VALUES(1, 'Data has been successfully updated', 'UPDATE', @IdUser, GETDATE())

				COMMIT TRANSACTION TUP
			END
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN 
			ROLLBACK TRANSACTION TUP
		END

		SET @ErrorMessage = ERROR_MESSAGE()
		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, @ErrorMessage, 'ERROR', @IdUser, GETDATE())
	END CATCH

	SET NOCOUNT OFF;
	SELECT * FROM @Result
END
GO

/** GET ALL **/
CREATE OR ALTER PROCEDURE
	Usp_Users_GetAll
AS
BEGIN
	SET NOCOUNT ON;
		DECLARE @Result AS TABLE
		(
			ResultStatus BIT,
			ResultMessage VARCHAR(100),
			OperationType VARCHAR(20),
			AffectedRecordId INT,
			OperationDateTime DATETIME,
			IdUser INT,
			IdRole INT,
			FirstName VARCHAR(20),
			LastName VARCHAR(20),
			Email VARCHAR(50),
			PasswordHash VARCHAR(100),
			CreationDate DATETIME,
			RegistrationConfirmedAt DATETIME,
			IsActive INT,
			IsRegistrationConfirmed INT
		)

		BEGIN TRY
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, 
				FirstName, LastName, Email, PasswordHash, CreationDate, RegistrationConfirmedAt, IsActive, IsRegistrationConfirmed)
			SELECT 1, 'User found', 'GET ALL', 0, GETDATE(), IdUser, IdRole, FirstName, LastName, Email, PasswordHash, CreationDate, RegistrationConfirmedAt, 
				IsActive, IsRegistrationConfirmed
			FROM Users
		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, 
				FirstName, LastName, Email, PasswordHash, CreationDate, RegistrationConfirmedAt, IsActive, IsRegistrationConfirmed)
			VALUES(0, @ErrorMessage, 'GET ALL', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
		END CATCH
	
	SET NOCOUNT OFF;

	SELECT * FROM @Result
END
GO


/** UPDATE **/
CREATE OR ALTER PROCEDURE
	Ups_Users_Update
		@IdUser INT,
		@IdRole INT,
		@FirstName NVARCHAR(20),
		@LastName NVARCHAR(20),
		@Email NVARCHAR(50),
		@IsRegistrationConfirmed BIT,
		@RegistrationConfirmedAt DATETIME,
		@IsActive BIT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Result AS TABLE
	(
		ResultStatus BIT,
		ResultMessage VARCHAR(100),
		OperationType VARCHAR(20),
		AffectedRecordId INT,
		OperationDateTime DATETIME
	)
	BEGIN TRY
		BEGIN TRANSACTION 

		IF NOT EXISTS(SELECT 1 FROM [dbo].[Users] WHERE IdUser = @IdUser)
		BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			SELECT 0, 'Error: user not found', 'NONE', NULL, GETDATE()

            ROLLBACK TRANSACTION;
            RETURN;
		END
		ELSE BEGIN
			UPDATE Users
				SET IdRole = @IdRole,
					FirstName = @FirstName,
					LastName = @LastName,
					Email = @Email,
					IsRegistrationConfirmed = @IsRegistrationConfirmed,
					RegistrationConfirmedAt = @RegistrationConfirmedAt,
					IsActive = @IsActive
			WHERE IdUser = @IdUser

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES (1, 'Data has been sucessfully updated', 'UPDATE', @IdUser, GETDATE())

			COMMIT TRANSACTION 
		END
	END TRY
	
	
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION 
		END

		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, ERROR_MESSAGE(), 'ERROR', @IdUser, GETDATE())

	END CATCH
	SET NOCOUNT OFF;
	
	SELECT * FROM @Result
END
GO

/** UPDATE **/
CREATE OR ALTER PROCEDURE
	Ups_UsersProfile_Update
		@IdUser INT,
		@FirstName NVARCHAR(20),
		@LastName NVARCHAR(20),
		@Email NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Result AS TABLE
	(
		ResultStatus BIT,
		ResultMessage VARCHAR(100),
		OperationType VARCHAR(20),
		AffectedRecordId INT,
		OperationDateTime DATETIME
	)
	BEGIN TRY
		BEGIN TRANSACTION 

		IF NOT EXISTS(SELECT 1 FROM [dbo].[Users] WHERE IdUser = @IdUser)
		BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			SELECT 0, 'Error: user not found', 'NONE', NULL, GETDATE()

            ROLLBACK TRANSACTION;
            RETURN;
		END
		ELSE BEGIN
			UPDATE Users
				SET 
					FirstName = @FirstName,
					LastName = @LastName,
					Email = @Email
			WHERE IdUser = @IdUser

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES (1, 'Data has been sucessfully updated', 'UPDATE', @IdUser, GETDATE())

			COMMIT TRANSACTION 
		END
	END TRY
	
	
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION 
		END

		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, ERROR_MESSAGE(), 'ERROR', @IdUser, GETDATE())

	END CATCH
	SET NOCOUNT OFF;
	
	SELECT * FROM @Result
END
GO

/** UPDATE PASSWORD **/
CREATE OR ALTER PROCEDURE
	Usp_UserPassword_Update
		@Email NVARCHAR(50),
		@PasswordHash NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Result AS TABLE
	(
		ResultStatus BIT,
		ResultMessage VARCHAR(100),
		OperationType VARCHAR(20),
		AffectedRecordId INT,
		OperationDateTime DATETIME
	)
	BEGIN TRY
		BEGIN TRANSACTION TrnxUpCategories

		IF NOT EXISTS(SELECT 1 FROM [dbo].[Users] WHERE Email = @Email)
		BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			SELECT 0, 'Error: user not found', 'NONE', NULL, GETDATE()

            ROLLBACK TRANSACTION;
            RETURN;
		END
		ELSE BEGIN
			UPDATE Users
				SET 
					PasswordHash = @PasswordHash
			WHERE Email = @Email 
			DECLARE @LastId INT = (SELECT SCOPE_IDENTITY())
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES (1, 'Data has been sucessfully updated', 'UPDATE', @LastId, GETDATE())

			COMMIT TRANSACTION 
		END
	END TRY
	
	
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION 
		END

		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, ERROR_MESSAGE(), 'ERROR', 0, GETDATE())

	END CATCH
	SET NOCOUNT OFF;
	
	SELECT * FROM @Result
END
GO

/** ACTIVE **/
CREATE OR ALTER 
	PROCEDURE
		Usp_Users_Activate
			@IdUser INT,
			@Status INT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ErrorMessage NVARCHAR(4000)
	DECLARE @Result AS TABLE
	(
		ResultStatus BIT, 
		ResultMessage NVARCHAR(100),
		OperationType NVARCHAR(20),
		AffectedRecordId INT, 
		OperationDateTime DATETIME
	);

	BEGIN TRY
		BEGIN TRANSACTION TUP

		IF NOT EXISTS(SELECT 1 FROM [dbo].[Users] WHERE IdUser = @IdUser)
		BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES (0, 'Error: User not found', 'NONE', NULL, GETDATE())

            ROLLBACK TRANSACTION;
            RETURN;
		END
		ELSE BEGIN
			UPDATE Users
				SET IsActive = @Status
			WHERE IdUser = @IdUser

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES( 1, 'Data has been successfully updated', 'UPDATE', @IdUser, GETDATE())

			COMMIT TRANSACTION 
		END
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION 
		END
		SET @ErrorMessage = ERROR_MESSAGE()
		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, @ErrorMessage, 'ERROR', @IdUser, GETDATE())
	END CATCH

	SET NOCOUNT OFF;
	SELECT * FROM @Result
END
GO

/** DELETE **/
CREATE OR ALTER PROCEDURE
	Usp_Users_Delete
		@IdUser INT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ErrorMessage NVARCHAR(4000) 
	DECLARE @Result AS TABLE
	(
		ResultStatus INT,
		ResultMessage VARCHAR(100),
		OperationType VARCHAR(20),
		AffectedRecordId INT, 
		OperationDateTime DATETIME
	)
	BEGIN TRY
		BEGIN TRANSACTION TDEL

			DELETE FROM Users
			WHERE IdUser = @IdUser

		IF @@ROWCOUNT = 0
		BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES(0, 'Error: User not found', 'NONE', @IdUser, GETDATE())

            ROLLBACK TRANSACTION;
            RETURN;
		END
		ELSE BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES(1, 'Data has been sucessfully deleted', 'DELETE', @IdUser, GETDATE())

			COMMIT TRANSACTION 
		END
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION 
		END

		SET @ErrorMessage = ERROR_MESSAGE()

		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, @ErrorMessage, 'NONE', @IdUser, GETDATE())
	END CATCH

	SET NOCOUNT OFF;

	SELECT * FROM @Result
END
