
--CREATE DATABASE OCC_Clone

USE [OCC_Clone]
GO

--;
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[SelectionProcess]'))
BEGIN
	ALTER TABLE [dbo].[SelectionProcess] DROP CONSTRAINT IF EXISTS [FK_SelectionProcess_Publications]
	ALTER TABLE [dbo].[SelectionProcess] DROP CONSTRAINT IF EXISTS [FK_SelectionProcess_Users]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Publications]'))
BEGIN
	ALTER TABLE [dbo].[Publications] DROP CONSTRAINT IF EXISTS [FK_Publications_Users]
	ALTER TABLE [dbo].[Publications] DROP CONSTRAINT IF EXISTS [FK_Publications_Roles]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Jobs]'))
BEGIN
	ALTER TABLE [dbo].[Jobs] DROP CONSTRAINT IF EXISTS [FK_Jobs_JobsTypes]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[SearchJobs]'))
BEGIN 
	ALTER TABLE [dbo].[SearchJobs] DROP CONSTRAINT IF EXISTS [FK_SearchJobs_JobTypes]
	ALTER TABLE [dbo].[SearchJobs] DROP CONSTRAINT IF EXISTS [FK_SearchJobs_Users]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID= OBJECT_ID(N'[dbo].[RefreshTokens]'))
BEGIN
	ALTER TABLE [dbo].[RefreshTokens] DROP CONSTRAINT IF EXISTS [FK_RefreshTokens_Users]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Users]'))
BEGIN
	ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [FK_Users_Roles]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[JobApplications]'))
BEGIN 
	ALTER TABLE [dbo].[JobApplications] DROP CONSTRAINT IF EXISTS [FK_JobApplications_Publications]
	ALTER TABLE [dbo].[JobApplications] DROP CONSTRAINT IF EXISTS [FK_JobApplications_Users] 
END
GO

--;
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[SelectionProcess]'))
BEGIN
	DROP TABLE [dbo].[SelectionProcess]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Publications]'))
BEGIN 
	DROP TABLE [dbo].[Publications]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Jobs]'))
BEGIN
	DROP TABLE [dbo].[Jobs]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[JobTypes]'))
BEGIN
	DROP TABLE [dbo].[JobTypes]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[SearchJobs]'))
BEGIN
	DROP TABLE [dbo].[SearchJobs]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Roles]'))
BEGIN
	DROP TABLE [dbo].[Roles]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[RefreshTokens]'))
BEGIN
	DROP TABLE [dbo].[RefreshTokens]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Users]'))
BEGIN
	DROP TABLE [dbo].[Users]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[IntegrationEvents]'))
BEGIN
	DROP TABLE [dbo].[IntegrationEvents] 
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[JobApplications]'))
BEGIN
	DROP TABLE [dbo].[JobApplications]
END
GO
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[EventDetailsLogs]'))
BEGIN 
	DROP TABLE [dbo].[EventDetailsLogs]
END
GO
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[ErrorDetailsLogs]'))
BEGIN 
	DROP TABLE [dbo].[ErrorDetailsLogs]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[ApplicationLogs]'))
BEGIN 
	DROP TABLE [dbo].[ApplicationLogs]
END 
GO

CREATE TABLE Roles
(
	IdRole INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	RoleName NVARCHAR(20) NOT NULL
)

CREATE TABLE Users
(
	IdUser INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	IdRole INT NOT NULL,
	FirstName NVARCHAR(20) NOT NULL,
	LastName NVARCHAR(20) NOT NULL,
	Email NVARCHAR(50) UNIQUE NOT NULL,
	PasswordHash NVARCHAR(100) NOT NULL,
	CreationDate DATETIME NOT NULL,
	IsActive BIT NOT NULL DEFAULT 0,
	IsRegistrationConfirmed BIT NOT NULL DEFAULT 0,
    RegistrationConfirmedAt DATETIME NULL,
)

ALTER TABLE Users
	WITH CHECK ADD CONSTRAINT [FK_Users_Roles]
	FOREIGN KEY([IdRole]) REFERENCES Roles([IdRole])


CREATE TABLE RefreshTokens
(
	IdRefreshToken INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	IdUser INT NOT NULL,
	Refresh_Token VARCHAR(512) NOT NULL,
	Expiration DATETIME NOT NULL,
	IssuedAt DATETIME DEFAULT GETDATE(),
	Revoked BIT DEFAULT 0,
	Device_Fingerprint VARCHAR(255)
)

ALTER TABLE RefreshTokens
	WITH CHECK ADD CONSTRAINT [FK_RefreshTokens_Users]
	FOREIGN KEY([IdUser]) REFERENCES Users([IdUser])

CREATE TABLE JobTypes
(
	IdJobType INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	JobTypeName NVARCHAR(50) NOT NULL
)

CREATE TABLE Publications
(
	IdPublication INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	IdUser INT NOT NULL,
	IdRole INT NOT NULL,
	Title NVARCHAR(100) NOT NULL,
	Description NVARCHAR(MAX),
	PublicationDate DATETIME NOT NULL,
	ExpirationDate DATETIME NOT NULL,
	Status TINYINT NOT NULL,
	Salary DECIMAL(10, 2) NULL,
	Location NVARCHAR(100) NULL,
	Company NVARCHAR(100) NULL,
	IdJobType INT NULL
)

ALTER TABLE Publications
	WITH CHECK ADD CONSTRAINT [FK_Publications_Users]
	FOREIGN KEY([IdUser]) REFERENCES Users([IdUser])
ALTER TABLE Publications
	WITH CHECK ADD CONSTRAINT [FK_Publications_Roles]
	FOREIGN KEY([IdRole]) REFERENCES Roles([IdRole])
ALTER TABLE Publications
	WITH CHECK ADD CONSTRAINT [FK_Publications_JobType]
	FOREIGN KEY([IdJobType]) REFERENCES JobTypes([IdJobType])

CREATE TABLE SelectionProcess
(
	IdSelectionProcess INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	IdPublication INT NOT NULL,
	IdApplicant INT NOT NULL,
	ApplicationStatus NVARCHAR(20) NOT NULL,
	ApplicationDate DATETIME NOT NULL
)

ALTER TABLE SelectionProcess
	WITH CHECK ADD CONSTRAINT [FK_SelectionProcess_Publications]
	FOREIGN KEY([IdPublication]) REFERENCES Publications([IdPublication])
ALTER TABLE SelectionProcess
	WITH CHECK ADD CONSTRAINT [FK_SelectionProcess_Users]
	FOREIGN KEY([IdApplicant]) REFERENCES Users([IdUser])
	   

CREATE TABLE JobApplications
(
	IdApplication INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	IdPublication INT NOT NULL,
	IdApplicant INT NOT NULL,
	ApplicantName NVARCHAR(100) NOT NULL,
	ApplicantResume NVARCHAR(MAX) NULL,
	CoverLetter NVARCHAR(MAX) NULL,
	ApplicationDate DATETIME NOT NULL DEFAULT GETDATE(),
	Status INT NULL DEFAULT 0,
	StatusMessage NVARCHAR(20) NOT NULL DEFAULT 'WithoutApplying'
)

ALTER TABLE [JobApplications]
	WITH CHECK ADD CONSTRAINT [FK_JobApplications_Publications]
	FOREIGN KEY([IdPublication]) REFERENCES Publications([IdPublication])

ALTER TABLE [JobApplications]
	WITH CHECK ADD CONSTRAINT [FK_JobApplications_Users]
	FOREIGN KEY([IdApplicant]) REFERENCES Users([IdUser])

CREATE TABLE SearchJobs
(
	IdSearch INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	IdUser INT NOT NULL,
	IdJobType INT NULL,
	SearchQuery NVARCHAR(255) NOT NULL,
	SearchDate DATETIME NOT NULL DEFAULT GETDATE(),
	Location NVARCHAR(100) NULL,
	MinSalary DECIMAL(10,2) NULL,
	MaxSalary DECIMAL(10,2) NULL,
)

ALTER TABLE SearchJobs
	WITH CHECK ADD CONSTRAINT [FK_SearchJobs_JobTypes]
	FOREIGN KEY([IdJobType]) REFERENCES JobTypes([IdJobType])
ALTER TABLE SearchJobs
	WITH CHECK ADD CONSTRAINT [FK_SearchJobs_Users]
	FOREIGN KEY([IdUser]) REFERENCES Users([IdUser])


CREATE TABLE IntegrationEvents (--;IntegrationEvents (
    IdEvent INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	IdCorrelation NVARCHAR(255),
    EventType NVARCHAR(100) NOT NULL,
    EventData NVARCHAR(MAX) NOT NULL,
    Exchange NVARCHAR(100) NOT NULL,
    RoutingKey NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE ApplicationLogs (--;[dbo].[ApplicationLogs] (
    IdLog INT IDENTITY(1,1) PRIMARY KEY,
    IdCorrelation NVARCHAR(255) NOT NULL, --UNIQUE, -- Identificador único para relacionar
	MicroserviceName NVARCHAR(100),
    Environment NVARCHAR(255),
    LogDate DATETIME NOT NULL DEFAULT GETDATE(),
    ExceptionMessage  NVARCHAR(MAX),
    LogLevel NVARCHAR(50),
    LoggerName NVARCHAR(255),
    Message NVARCHAR(MAX),
    ExceptionStackTrace NVARCHAR(MAX),
    ThreadId NVARCHAR(50)
);
CREATE TABLE EventDetailsLogs (--;[dbo].[EventDetailsLogs] (
    IdDetailLog INT IDENTITY(1,1) PRIMARY KEY,
    IdCorrelation NVARCHAR(255) NOT NULL, -- Relación con ApplicationLogs
    MethodName NVARCHAR(255),
    CallSite NVARCHAR(MAX),
    MachineName NVARCHAR(255),
    Username NVARCHAR(255),
    --FOREIGN KEY (IdCorrelation) REFERENCES ApplicationLogs (IdCorrelation) ON DELETE CASCADE
);
CREATE TABLE ErrorDetailsLogs (--;[dbo].[ErrorDetailsLogs] (
    IdErrorLog INT IDENTITY(1,1) PRIMARY KEY,
    IdCorrelation NVARCHAR(255) NOT NULL, -- Relación con ApplicationLogs
    ServerName NVARCHAR(255),
    LineNumber INT,
    FileName NVARCHAR(255),
    LogDate DATETIME NOT NULL DEFAULT GETDATE(),
    --FOREIGN KEY (IdCorrelation) REFERENCES ApplicationLogs (IdCorrelation) ON DELETE CASCADE
);



INSERT INTO JobTypes (JobTypeName) VALUES
	('Backend Developer'),
	('Frontend Developer'),
	('Full Stack Developer'),
	('Mobile Developer'),
	('DevOps Engineer'),
	('Cloud Engineer'),
	('Data Scientist'),
	('Data Analyst'),
	('Machine Learning Engineer'),
	('Systems Administrator'),
	('Database Administrator'),
	('Security Engineer'),
	('Technical Lead'),
	('Engineering Manager'),
	('Product Manager'),
	('UI/UX Designer'),
	('Solutions Architect'),
	('IT Consultant'),
	('Technical Support Specialist');

INSERT INTO Roles(RoleName) 
VALUES ('Admin'),('Recruiter'),('Applicant');

--; passHash nUrT/5WpKOi/inSmWr9jgtjItwEYp8gDgwgEXtVy2JokVbhxTT5vDVxYCmbRNj8H
--; pass 12341234
INSERT INTO Users(IdRole, FirstName, LastName, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed, RegistrationConfirmedAt)
VALUES	(2,'Dutch','S','dutch@fake.com','4PWcJWpd1B+U2JDnLwetZRLWuianGLodczIwjS04j6dKejHpE5N6A3ieZ9+t8D5c',GETDATE(),0,0, NULL),	--; recruiter
		(2,'Sigrid','S','sigrid@fake.com','ywa5iLldCBHMLlHViA7xILjoQ9dYSnCtIZy4fTfmKm10xKsDMfWV1LbiTH0u64rX',GETDATE(),0,0, NULL),	--; recruiter
		(2,'Helga','S','helga@fake.com','wh57hxfW7wjSONdacX2YZRxyV/P4YDVGeTnmTjtUiBYr9y3Zu6SBbOMhbg9pnV+2',GETDATE(),0,0, NULL),	--; recruiter
		(3,'Tula','S','tula@fake.com','fvMPuTmT1Km0HYCcrusrOIg/UEqx9xIYIFO2FLbdH23CQYWB6jQY1ZDESA/juk0s',GETDATE(),0,0, NULL),	--; applicant
		(3,'Thor','S','thor@fake.com','k+s9AOLmbP3GWCfxO0h3tUgENJppbKk5V7NHnkuwVhSTovffajcKSmgdL6l56J75',GETDATE(),0,0, NULL),	--; applicant
		(3,'Loki','S','loki@fake.com','qzC8RXaRuR97qAEqaAeTyQMDS2cdmDoPj+cg1mjlJG2MQZIVzbw8Elm4FWYj8QrO',GETDATE(),0,0, NULL);	--; applicant

INSERT INTO Publications( IdUser, IdRole, Title, Description, PublicationDate, ExpirationDate, Status, Salary, Location, Company, IdJobType)
VALUES(1,2, 'Backend Developer Position', 'Looking ofr an experienced Backend Developer with knowledge of .NET', GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 21000.00, 'Mexico City', 'OCP',1),
	(1,2, 'Frontend Developer Position', 'Seeking a skilled Frontend Developer with experience in Angular and TypeScript', GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 70000.00, 'San Francisco', 'OCP',2),
	(1,2, 'Full Stack Developer (.NET & C#)', 'Hybrid  Full-time  Associate', GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 18500.00, 'Mexico City', 'OCP', 3),
	(1,2, 'Software Engineer II', 'Remote  Full-time', GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 21100.00, 'Mexico City', 'OCP', 1),
	(1,2, 'Deployment Engineer', 'On-site  Contract  Mid-Senior level', GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 6500.00, 'Mexico City', 'OCP', 1),
	(1,2, 'Java Developer (Angular) ', 'Remote  Full-time  Mid-Senior level', GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 9500.00, 'Mexico City', 'OCP', 3),
	(1,2, 'Data Quality Assurance Engineer', 'Remote  Full-time  Mid-Senior level', GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 8500.00, 'Mexico City', 'OCP', 8),
	(1,2, 'DevOps Engineer', 'Remote  Full-time  Mid-Senior level', GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 2500.00, 'Mexico City', 'OCP', 5),
	(1,2, 'Full Stack Engineer -.NET ** React *Remote', 'Remote  Full-time  Mid-Senior level', GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 13500.00, 'Mexico City', 'OCP', 3)

/**
DECLARE @MinSalary DECIMAL = 10000
DECLARE @MaxSalary DECIMAL = 30000
DECLARE @Location VARCHAR(100) = 'Mexico City'
DECLARE @JobType NVARCHAR(100) = 'Backend Developer'
DECLARE @Keyword NVARCHAR(100) = 'Backend'

SELECT p.IdPublication, p.Title, p.Description, p.Salary, p.Location, t.JobTypeName, p.Company
FROM Publications p (NOLOCK)
INNER JOIN 
	JobTypes t ON p.IdJobType = t.IdJobType
WHERE (p.Salary BETWEEN @MinSalary AND @MaxSalary OR (@MinSalary IS NULL AND @MaxSalary IS NULL))
	AND (p.Location = @Location OR @Location IS NULL)
	AND (t.JobTypeName = @JobType OR @JobType IS NULL)
	AND (p.Title LIKE '%' + @Keyword + '%' OR @Keyword IS NULL)
	AND p.Status = 1;
	**/

INSERT INTO SelectionProcess(IdPublication, IdApplicant, ApplicationStatus, ApplicationDate)
VALUES(1, 2, 'Applied', GETDATE());

INSERT INTO SearchJobs(IdUser, IdJobType, SearchQuery, SearchDate, Location, MinSalary, MaxSalary)
VALUES(2, 1, 'Backend', GETDATE(), 'Mexico City', 10000, 30000);

