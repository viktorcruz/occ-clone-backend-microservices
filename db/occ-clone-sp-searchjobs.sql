USE [OCC_CLONE]
GO

IF OBJECT_ID('[dbo].[Usp_JobApplications_Add]', 'P') IS NOT NULL
BEGIN 
	DROP PROCEDURE [dbo].[Usp_JobApplications_Add]
END
GO
IF OBJECT_ID('[dbo].[Usp_JobApplications_GetAll]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_JobApplications_GetAll]
END
GO
IF OBJECT_ID('[dbo].[Usp_JobApplications_Withdraw]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_JobApplications_Withdraw]
END
GO
IF OBJECT_ID('[dbo].[Usp_JobApplications_Search]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_JobApplications_Search]
END
GO

/** INSERT **/
CREATE OR ALTER PROCEDURE
	Usp_JobApplications_Add
		@IdPublication INT,
		@IdApplicant INT, 
		@ApplicantName NVARCHAR(MAX),
		@ApplicantResume NVARCHAR(MAX),
		@CoverLetter NVARCHAR(MAX),
		@ApplicationDate DATETIME,
		@Status NVARCHAR(20)
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

			INSERT INTO [dbo].[JobApplications](IdPublication, IdApplicant, ApplicantName, ApplicantResume, CoverLetter, Status)
			VALUES(@IdPublication, @IdApplicant, @ApplicantName, @ApplicantResume, @CoverLetter, @Status)
			
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

CREATE OR ALTER   PROCEDURE
	[dbo].[Usp_JobApplications_GetAll]
		@IdApplicant INT
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
		OperationDate DATETIME,
		IdPublication INT,
		IdApplicant INT, 
		ApplicantName NVARCHAR(MAX),
		ApplicantResume NVARCHAR(MAX),
		CoverLetter NVARCHAR(MAX),
		ApplicationDate DATETIME,
		Status NVARCHAR(20)
	);

	BEGIN TRY
		IF EXISTS(SELECT 1 FROM [dbo].[JobApplications] WHERE IdApplicant = @IdApplicant)
		BEGIN
			INSERT INTO @Result
			(
				ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDate, 
				IdPublication, IdApplicant, ApplicantName, ApplicantResume, CoverLetter, ApplicationDate, Status
			)
			SELECT 1, 'Application found', 'GET', 0, GETDATE(), IdPublication, IdApplicant,
				ApplicantName, ApplicantResume, CoverLetter, ApplicationDate, Status
			FROM [dbo].[JobApplications]
			WHERE IdApplicant = @IdApplicant
		END
		ELSE BEGIN
			INSERT INTO @Result
			(
				ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDate, 
				IdPublication, IdApplicant, ApplicantName, ApplicantResume, CoverLetter, ApplicationDate, Status
			)
			VALUES(0, 'Error: Application not found', 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL)
		END
	END TRY

	BEGIN CATCH
		SET @ErrorMessage = ERROR_MESSAGE()
		INSERT INTO @Result
		(
			ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDate, 
			IdPublication, IdApplicant, ApplicantName, ApplicantResume, CoverLetter, ApplicationDate, Status
		)
		VALUES(0, @ErrorMessage, 'GET', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL)
	END CATCH

	SET NOCOUNT OFF;
	SELECT * FROM @Result
END
GO

CREATE OR ALTER PROCEDURE Usp_JobApplications_Withdraw
    @IdApplicant INT,
    @IdPublication INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @Result AS TABLE
    (
        ResultStatus BIT,
        ResultMessage NVARCHAR(100),
        OperationType NVARCHAR(20),
        AffectedRecordId INT,
        OperationDateTime DATETIME
    );

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[JobApplications] WHERE IdApplicant = @IdApplicant AND IdPublication = @IdPublication)
        BEGIN 
            INSERT INTO @Result (ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
            VALUES (0, 'Error: applicant not found', 'UPDATE', NULL, GETDATE());

            ROLLBACK TRANSACTION;
            RETURN;
        END
        ELSE 
        BEGIN
            UPDATE JobApplications
            SET Status = 'withdrawn'
            WHERE IdApplicant = @IdApplicant AND IdPublication = @IdPublication;

            INSERT INTO @Result (ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
            VALUES (1, 'Data has been successfully updated', 'UPDATE', @IdApplicant, GETDATE());

            COMMIT TRANSACTION;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END
        
        SET @ErrorMessage = ERROR_MESSAGE();

        INSERT INTO @Result (ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime)
        VALUES (0, @ErrorMessage, 'ERROR', @IdApplicant, GETDATE());
    END CATCH

    SET NOCOUNT OFF;
    
	SELECT * FROM @Result;
END;
GO


CREATE OR ALTER PROCEDURE
	Usp_JobApplications_Search
		@Keyword NVARCHAR(20) = NULL
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
		OperationDateTime DATETIME,
		IdPublication INT,
		IdJob INT,
		JobTypeName VARCHAR(100),
		Title NVARCHAR(100),
		Description NVARCHAR(100),
		Company NVARCHAR(100),
		Location NVARCHAR(100),
		Salary DECIMAL,
		ExpirationDate DATETIME,
		PostedDate DATETIME
	);

	BEGIN TRY
		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdPublication, IdJob, JobTypeName,
					Title, Description, Company, Location, Salary, ExpirationDate, PostedDate)
		SELECT 1, 'Job found', 'GET ALL', 0, GETDATE(), p.IdPublication, t.IdJobType, t.JobTypeName, p.Title, p.Description, p.Company, p.Location, 
					p.Salary, p.ExpirationDate, p.PublicationDate AS PostedDate
		FROM Publications p
		LEFT JOIN
			JobTypes t ON p.IdJobType = t.IdJobType
		WHERE
		(
			@Keyword IS NULL OR 
			p.Title LIKE '%' + @Keyword + '%' 
			OR p.Description LIKE '%' + @Keyword + '%' 
			OR p.Location LIKE '%' + @Keyword + '%'
			OR t.JobTypeName LIKE '%' + @Keyword + '%'
			OR p.Company LIKE '%' + @Keyword + '%'
		)
	END TRY
	BEGIN CATCH
		SET @ErrorMessage = ERROR_MESSAGE()

		INSERT INTO @Result(ResultStatus, ResultMessage, OperationType, AffectedRecordId, OperationDateTime, IdPublication, IdJob, JobTypeName,
					Title, Description, Company, Location, Salary, ExpirationDate, PostedDate)
		VALUES(0, @ErrorMessage, 'SEARCH', 0, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
	END CATCH

	SET NOCOUNT OFF;
	SELECT * FROM @Result;
END
GO





