

USE [OCC_CLONE]
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



/** INSERT **/
CREATE OR ALTER PROCEDURE
	Usp_Users_Add
		@IdRole INT,
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

		IF EXISTS(SELECT 1 FROM [dbo].[Users] WHERE Email = @Email)
		BEGIN 
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			SELECT 0, 'Error: User already exists', 'NONE', NULL, GETDATE()

			ROLLBACK TRANSACTION 
		END
		ELSE BEGIN
			INSERT INTO [dbo].[Users](idRole, FirstName, LastName, Email, CreationDate)
			VALUES(@IdRole, @FirstName, @LastName, @Email, GETDATE())

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
		@idUser INT
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
			CreationDate DATETIME	
		)

		BEGIN TRY
			IF EXISTS( SELECT 1 FROM Users (nolock) WHERE idUser = @idUser )
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, FirstName, LastName, Email, CreationDate)
				SELECT 1, 'User found', 'GET', 0, GETDATE(), IdUser, idRole, FirstName, LastName, Email, CreationDate
				FROM Users (nolock)  
				WHERE idUser = @idUser

			END
			ELSE
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, FirstName, LastName, Email, CreationDate)
				VALUES(0, 'Error: User not found', 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL)
			END
		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, FirstName, LastName, Email, CreationDate)
			VALUES(0, @ErrorMessage, 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL)
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
			CreationDate DATETIME
		)

		BEGIN TRY
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, FirstName, LastName, Email, CreationDate )
			SELECT 1, 'User found', 'GET ALL', 0, GETDATE(), IdUser, IdRole, FirstName, LastName, Email, CreationDate
			FROM Users
		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdUser, IdRole, FirstName, LastName, Email, CreationDate )
			VALUES(0, @ErrorMessage, 'GET ALL', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL)
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
		BEGIN TRANSACTION TrnxUpCategories

		IF NOT EXISTS(SELECT 1 FROM [dbo].[Users] WHERE IdUser = @IdUser)
		BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			SELECT 0, 'Error: caterory not found', 'NONE', NULL, GETDATE()

			ROLLBACK TRANSACTION TrnxUpCategories
		END
		ELSE BEGIN
			UPDATE Users
				SET IdRole = @IdRole,
					FirstName = @FirstName,
					LastName = @LastName,
					Email = @Email
			WHERE IdUser = @IdUser

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES (1, 'Data has been sucessfully updated', 'UPDATE', @IdUser, GETDATE())

			COMMIT TRANSACTION TrnxUpCategories
		END
	END TRY
	
	
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION TrnUpdating
		END

		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, ERROR_MESSAGE(), 'ERROR', @IdUser, GETDATE())

	END CATCH
	SET NOCOUNT OFF;
	
	SELECT * FROM @Result
END
GO


/** DELETE **/
CREATE OR ALTER PROCEDURE
	Usp_Users_Delete
		@idUser INT
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
			WHERE idUser = @idUser

		IF @@ROWCOUNT = 0
		BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES(0, 'Error: User not found', 'NONE', @idUser, GETDATE())

			ROLLBACK TRANSACTION TDEL
		END
		ELSE BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES(1, 'Data has been sucessfully deleted', 'DELETE', @idUser, GETDATE())

			COMMIT TRANSACTION TDEL
		END
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION TDEL
		END

		SET @ErrorMessage = ERROR_MESSAGE()

		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, @ErrorMessage, 'NONE', @idUser, GETDATE())
	END CATCH

	SET NOCOUNT OFF;

	SELECT * FROM @Result
END

/**

EXEC Usp_Users_Add @Name = 'User 1'
EXEC Ups_Users_Update @CategoryId = 1, @Name = 'User 2'
EXEC Usp_Users_Get @idUser = 4
EXEC Usp_Users_GetAll
EXEC Usp_Users_Delete @idUser = 3


INSERT INTO Roles(RoleName) VALUES('Recuiter'), ('Applicant')

**/