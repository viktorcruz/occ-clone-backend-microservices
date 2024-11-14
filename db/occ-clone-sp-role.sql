

USE [OCC_Clone]
GO

IF OBJECT_ID('[dbo].[Usp_Roles_Add]', 'P') IS NOT NULL
BEGIN 
	DROP PROCEDURE [dbo].[Usp_Roles_Add]
END
GO
IF OBJECT_ID('[dbo].[Usp_Roles_Get]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_Roles_Get]
END
GO
IF OBJECT_ID('[dbo].[Usp_Roles_GetAll]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_Roles_GetAll]
END
GO
IF OBJECT_ID('[dbo].[Ups_Roles_Update]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Ups_Roles_Update]
END
GO
IF OBJECT_ID('[dbo].[Usp_Roles_Delete]', 'P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_Roles_Delete]
END
GO


/** INSERT **/
CREATE OR ALTER PROCEDURE
	Usp_Roles_Add
		@RoleName NVARCHAR(20)
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

		IF EXISTS(SELECT 1 FROM [dbo].[Roles] WHERE RoleName = @RoleName)
		BEGIN 
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			SELECT 0, 'Error: RoleName already exists', 'INSERT', NULL, GETDATE()

			ROLLBACK TRANSACTION 
		END
		ELSE BEGIN
			INSERT INTO [dbo].[Roles](RoleName)
			VALUES(@RoleName)

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
		VALUES(0, ERROR_MESSAGE(), 'INSERT', NULL, GETDATE())

	END CATCH
	
	SET NOCOUNT OFF;
	SELECT * FROM @Result
END
GO


/** GET **/
CREATE OR ALTER PROCEDURE
	Usp_Roles_Get
		@IdRole INT
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
			IdRole INT,
			RoleName VARCHAR(20)
		)

		BEGIN TRY
			IF EXISTS( SELECT 1 FROM Roles (nolock) WHERE IdRole = @IdRole )
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdRole, RoleName)
				SELECT 1, 'Role found', 'GET', 0, GETDATE(), IdRole, RoleName
				FROM Roles (nolock)  
				WHERE IdRole = @IdRole

			END
			ELSE
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdRole, RoleName)
				VALUES(0, 'Error: User not found', 'GET', 0, GETDATE(), NULL, NULL)
			END
		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdRole, RoleName)
			VALUES(0, @ErrorMessage, 'GET', 0, GETDATE(), NULL, NULL)
		END CATCH
	
	SET NOCOUNT OFF;

	SELECT * FROM @Result
END
GO


/** GET ALL **/
CREATE OR ALTER PROCEDURE
	Usp_Roles_GetAll
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
			IdRole INT,
			RoleName VARCHAR(20)
		)

		BEGIN TRY
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdRole, RoleName)
			SELECT 1, 'User found', 'GET ALL', 0, GETDATE(), IdRole, RoleName
			FROM Roles
		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdRole, RoleName)
			VALUES(0, @ErrorMessage, 'GET ALL', 0, GETDATE(), NULL, NULL)
		END CATCH
	
	SET NOCOUNT OFF;

	SELECT * FROM @Result
END
GO


/** UPDATE **/
CREATE OR ALTER PROCEDURE
	Ups_Roles_Update
		@IdRole INT,
		@RoleName NVARCHAR(50)
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

		IF NOT EXISTS(SELECT 1 FROM [dbo].[Roles] WHERE IdRole = @IdRole)
		BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			SELECT 0, 'Error: Role not found', 'UPDATE', NULL, GETDATE()

			ROLLBACK TRANSACTION TrnxUpCategories
		END
		ELSE BEGIN
			UPDATE Roles
				SET RoleName = @RoleName
			WHERE IdRole = @IdRole

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES (1, 'Data has been sucessfully updated', 'UPDATE', @IdRole, GETDATE())

			COMMIT TRANSACTION TrnxUpCategories
		END
	END TRY
	
	
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION TrnUpdating
		END

		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
		VALUES(0, ERROR_MESSAGE(), 'UPDATE', @IdRole, GETDATE())

	END CATCH
	SET NOCOUNT OFF;
	
	SELECT * FROM @Result
END
GO


/** DELETE **/
CREATE OR ALTER PROCEDURE
	Usp_Roles_Delete
		@IdRole INT
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

			DELETE FROM Roles
			WHERE IdRole = @IdRole

		IF @@ROWCOUNT = 0
		BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES(0, 'Error: Role not found', 'DELETE', @IdRole, GETDATE())

			ROLLBACK TRANSACTION TDEL
		END
		ELSE BEGIN
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES(1, 'Data has been sucessfully deleted', 'DELETE', @IdRole, GETDATE())

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
		VALUES(0, @ErrorMessage, 'DELETE', @IdRole, GETDATE())
	END CATCH

	SET NOCOUNT OFF;

	SELECT * FROM @Result
END

/**

EXEC Usp_Roles_Add @RoleName = 'User 1'
EXEC Ups_Roles_Update @IdRole = 1, @RoleName = 'User 2'
EXEC Usp_Roles_Get @IdRole = 4
EXEC Usp_Roles_GetAll
EXEC Usp_Roles_Delete @IdUser = 3


INSERT INTO Roles(RoleName) VALUES('Recuiter'), ('Applicant')

**/