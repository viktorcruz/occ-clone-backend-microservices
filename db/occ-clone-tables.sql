
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
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID= OBJECT_ID(N'[dbo].[RefreshTokens]'))
BEGIN
	ALTER TABLE [dbo].[RefreshTokens] DROP CONSTRAINT IF EXISTS [FK_RefreshTokens_Users]
END
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Users]'))
BEGIN
	ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [FK_Users_Roles]
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


CREATE TABLE Publications
(
	idPublication INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	idRecruiter INT NOT NULL,
	Title NVARCHAR(20) NOT NULL,
	Description NVARCHAR(100),
	PublicationDate DATETIME NOT NULL,
	IdRole INT NOT NULL,
	Status BIT NOT NULL,
	ExpirationDate DATETIME NOT NULL
)

ALTER TABLE Publications
	WITH CHECK ADD CONSTRAINT [FK_Publications_Users]
	FOREIGN KEY([idRecruiter]) REFERENCES Users([IdUser])
ALTER TABLE Publications
	WITH CHECK ADD CONSTRAINT [FK_Publications_Roles]
	FOREIGN KEY([IdRole]) REFERENCES Roles([IdRole])

CREATE TABLE SelectionProcess
(
	idSelectionProcess INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	idPublication INT NOT NULL,
	idApplicant INT NOT NULL,
	Status NVARCHAR(20) NOT NULL,
	ApplicationDate DATETIME NOT NULL
)

ALTER TABLE SelectionProcess
	WITH CHECK ADD CONSTRAINT [FK_SelectionProcess_Publications]
	FOREIGN KEY([idPublication]) REFERENCES Publications([idPublication])
ALTER TABLE SelectionProcess
	WITH CHECK ADD CONSTRAINT [FK_SelectionProcess_Users]
	FOREIGN KEY([idApplicant]) REFERENCES Users([IdUser])
	   


INSERT INTO Roles(RoleName) VALUES('Recruiter'),('Applicant')

--INSERT INTO Users(IdRole, FirstName, LastName, Email, PasswordHash, CreationDate, IsActive, IsRegistrationConfirmed, RegistrationConfirmedAt)
--VALUES	(2,'Maxx','Winter','the.maxx@fake.com','12341234',GETDATE(),0,0, GETDATE())
		--(1,'Karla','Fuentes Nolasco','karla.fuente@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Oscar','Kala Haak','oscar.kala@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Maria','Torres','maria.torres@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Xaime','Weir Rojo','xaime.rojo@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Adriana','Fernandez','adriana.fernandez@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Ingrid','Xodar Jimenez','ingrid.xodar@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Berenice','Ximo Quezada','berenice.ximo@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Rodrigo','Kitia Castro','rodrigo.kitia@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Vladimir','Nikolaev Ivanov','vladimir.ivanov@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Greta','Baumann Richter','greta.richter@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Jessica','Johnson Castillo','jessica.johnson@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Anastacia','Sokilova Braun','anastacia.braun@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Michel','Johnsson Figueroa','michel.johnsson@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Castro','Kennedy Schmitt','castro.kennedy@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Jorge','Lopez Fernandez','jorge.lopez@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Klaus','Schneider Wlaker','klaus.schneider@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Sandra','Connor','sandra.connor@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Clara','Luciani Le','clara.le@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'William','Wisher Jr','william.wisher@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Franco','Columbu Bess','franco.columbu@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Anna','Romanova Kuznetsova','anna.romanova@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Fritz','Fisher Weber','fritz.fisher@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Maria','Garcia Lopez','maria.garcia@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Olga','Hernandez Ramirez','olga.hernandez@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Antonio','Ramirez Acu a','antonio.ramirez@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Emilio','Gonzales','emilio.gonzales@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Sofia','Salazar Hernandez','sofia.salazar@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Hans','Mueller Schmidt','hans.mueller@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'John','Cena Johnson','john.cena@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Rosa','Delgadillo Paredes','rosa.delgadillo@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Pedro','Aguilar Rojas','pedro.agular@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Alejandro','Rivera Vargas','alejandro.rivera@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Jose','Alfredo Jimenez','jose.alfredo@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Elena','Mendoza Castro','elena.mendoza@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Lucia','Rivera Garcia','lucia.rivera@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Laura','Morales Dias','laura.dias@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Wagner','Mozart Bach','wagner.mozart@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Darwin','Walker','darwin.walker@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Anna','petrova Ivanova','anna.petrova@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Jessica','Kinolev Smith','jessica.kinolev@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Ivano','Thompson Weber','ivano.thompson@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Elena','Romero Morales','elena.romero@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(2,'Juan','Hernandez Hernandez','juan.hernandez@fake.com','12341234',GETDATE(),0,0, GETDATE()),
		--(1,'Sofia','Silva','sofia.silva@fake.com','12341234',GETDATE(),0,0, GETDATE())
	