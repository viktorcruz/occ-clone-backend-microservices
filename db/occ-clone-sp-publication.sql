USE [OCC_CLONE]
GO

/** INSERT **/
CREATE OR ALTER PROCEDURE
	Usp_Publications_Add
		@IdUser INT, 
		@IdRole INT, 
		@Title NVARCHAR(50),
		@Description NVARCHAR(255),
		@ExpirationDate DATETIME,
		@Status INT,
		@Salary DECIMAL,
		@Location NVARCHAR(100),
		@Company NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Result AS TABLE
	(
		ResultStatus BIT,
		ResultMessage NVARCHAR(100),
		OperationType NVARCHAR(20),
		AffectedRecordId INT, 
		OperationDateTime DATETIME
	)
	
	BEGIN TRY
		BEGIN TRANSACTION

			INSERT INTO [dbo].[Publications](IdUser, IdRole, Title, Description, PublicationDate, ExpirationDate, Status, Salary, Location, Company)
			VALUES(@IdUser, @IdRole, @Title, @Description, GETDATE(), @ExpirationDate, @Status, @Salary, @Location, @Company)
			
			DECLARE @LastId INT = (SELECT SCOPE_IDENTITY())
			
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
			VALUES(1, 'Data has been successfuly inserted', 'INSERT', @LastId, GETDATE())
			
			COMMIT TRANSACTION

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
	Usp_Publications_Get
		@IdPublication INT
AS 
BEGIN
	SET NOCOUNT ON;
		
		DECLARE @Result AS TABLE
		(
			ResultStatus BIT, 
			ResultMessage NVARCHAR(100),
			OperationType NVARCHAR(20),
			AffectedRecordId INT, 
			OperationDateTime DATETIME,
			IdPublication INT,
			IdUser INT, 
			IdRole INT,
			Title NVARCHAR(50),
			PublicationDate DATETIME,
			Description NVARCHAR(255),
			ExpirationDate DATETIME,
			Status INT,
			Salary DECIMAL,
			Location NVARCHAR(100),
			Company NVARCHAR(100)
		);

		BEGIN TRY
			IF EXISTS(SELECT 1 FROM Publications (NOLOCK) WHERE IdPublication = @IdPublication)
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdPublication, IdUser, IdRole, Title, Description, PublicationDate, ExpirationDate, Status, Salary, Location, Company)
				SELECT 1, 'Publication found', 'GET', 0, GETDATE(), IdPublication, IdUser, IdRole, Title, Description, PublicationDate, ExpirationDate, Status, Salary, Location, Company
				FROM [dbo].[Publications] (NOLOCK)
				WHERE IdPublication = @IdPublication
			END
			ELSE BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdPublication, IdUser, IdRole, Title, Description, PublicationDate, ExpirationDate, Status, Salary, Location, Company)
				VALUES(0, 'Error: Publication not found', 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
			END
		END TRY

		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdPublication, IdUser, IdRole, Title, Description, PublicationDate, ExpirationDate, Status, Salary, Location, Company)
			VALUES(0, @ErrorMessage, 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
		END CATCH

	SET NOCOUNT OFF;
	SELECT * FROM @Result
END
GO


/** GET ALL **/
CREATE OR ALTER PROCEDURE
	Usp_Publications_GetAll
AS
BEGIN
	SET NOCOUNT ON;

		DECLARE @Result AS TABLE
		(
			ResultStatus BIT,
			ResultMessage NVARCHAR(100),
			OperationType NVARCHAR(20),
			AffectedRecordId INT,
			OperationDateTime DATETIME,
			IdPublication INT,
			IdUser INT, 
			IdRole INT, 
			Title NVARCHAR(50),
			PublicationDate DATETIME,
			Description NVARCHAR(255),
			ExpirationDate DATETIME,
			Status INT,
			Salary DECIMAL,
			Location NVARCHAR(100),
			Company NVARCHAR(100)
		);

		BEGIN TRY
			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdPublication, IdUser, IdRole, Title, Description, PublicationDate, ExpirationDate, Status, Salary, Location, Company)
			SELECT 1, 'Publication found', 'GET ALL', 0, GETDATE(), IdPublication, IdUser, IdRole, Title, Description, PublicationDate, ExpirationDate, Status, Salary, Location, Company
			FROM [dbo].[Publications]
		END TRY

		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000)
			SET @ErrorMessage = ERROR_MESSAGE()

			INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdPublication, IdUser, IdRole, Title, Description, PublicationDate, ExpirationDate, Status, Salary, Location, Company)
			VALUES(0, @ErrorMessage, 'GET ALL', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
		END CATCH

	SET NOCOUNT OFF;
	SELECT * FROM @Result
END
GO

CREATE OR 
	ALTER PROCEDURE
		Usp_Publications_Update
			@IdPublication INT,
			@IdUser INT, 
			@IdRole INT, 
			@Description NVARCHAR(255),
			@Status INT, 
			@Salary DECIMAL,
			@Location NVARCHAR(100),
			@Company NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ErrorMessage NVARCHAR(4000)
	DECLARE @Result AS TABLE
	(
		ResultStatus INT, 
		ResultMessage NVARCHAR(100),
		OperationType NVARCHAR(20),
		AffectedRecordId INT,
		OperationDateTime DATETIME
	)
	BEGIN TRY
		BEGIN TRANSACTION TUP
			IF NOT EXISTS(SELECT 1 FROM Publications WHERE IdPublication = @IdPublication)
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
				VALUES (0, 'Error: Publication not found', 'NONE', NULL, GETDATE())

				ROLLBACK TRANSACTION TUP
				RETURN
			END
			ELSE BEGIN
				UPDATE Publications
					SET Description = @Description,
						Status = @Status,
						Salary = @Salary,
						Location = @Location,
						Company = @Company
					WHERE IdUser = @IdUser AND IdRole = @IdRole AND IdPublication = @IdPublication

				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
				VALUES ( 1, 'Data has been successfully updated', 'UPDATE', @IdPublication, GETDATE())

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
		VALUES (0, @ErrorMessage, 'ERROR', @IdPublication, GETDATE())

	END CATCH

	SET NOCOUNT OFF;
	SELECT * FROM @Result
END


/** DELETE **/
CREATE OR 
	ALTER PROCEDURE
	Usp_Publications_Delete
		@IdPublication INT
AS
BEGIN
	SET NOCOUNT ON;

		DECLARE @ErrorMessage NVARCHAR(4000)
		DECLARE @Result AS TABLE
		(
			ResultStatus INT,
			ResultMessage NVARCHAR(100),
			OperationType NVARCHAR(20),
			AffectedRecordId INT,
			OperationDateTime DATETIME
		)
		BEGIN TRY
			BEGIN TRANSACTION TDEL
				DELETE FROM Publications
				WHERE IdPublication = @IdPublication
			
			IF @@ROWCOUNT = 0
			BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
				VALUES(0, 'Error: Publication not found', 'NONE', @IdPublication, GETDATE())

				ROLLBACK TRANSACTION TDEL
			END
			ELSE BEGIN
				INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
				VALUES(1, 'Data has been successfully deleted', 'DELETE', @IdPublication, GETDATE())

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
			VALUES(0, @ErrorMessage, 'NONE', @IdPublication, GETDATE())
		END CATCH

	SET NOCOUNT OFF;
	SELECT * FROM @Result
END