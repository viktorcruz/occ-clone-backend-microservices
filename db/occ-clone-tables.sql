
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
IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Users]'))
BEGIN
	DROP TABLE [dbo].[Users]
END

GO

CREATE TABLE Roles
(
	idRole INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	RoleName NVARCHAR(20) NOT NULL
)

CREATE TABLE Users
(
	idUser INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	idRole INT NOT NULL,
	FirstName NVARCHAR(20) NOT NULL,
	LastName NVARCHAR(20) NOT NULL,
	Email NVARCHAR(50) UNIQUE NOT NULL,
	CreationDate DATETIME NOT NULL
)

ALTER TABLE Users
	WITH CHECK ADD CONSTRAINT [FK_Users_Roles]
	FOREIGN KEY([idRole]) REFERENCES Roles([idRole])


CREATE TABLE Publications
(
	idPublication INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	idRecruiter INT NOT NULL,
	Title NVARCHAR(20) NOT NULL,
	Description NVARCHAR(100),
	PublicationDate DATETIME NOT NULL,
	idRole INT NOT NULL,
	Status BIT NOT NULL,
	ExpirationDate DATETIME NOT NULL
)

ALTER TABLE Publications
	WITH CHECK ADD CONSTRAINT [FK_Publications_Users]
	FOREIGN KEY([idRecruiter]) REFERENCES Users([idUser])
ALTER TABLE Publications
	WITH CHECK ADD CONSTRAINT [FK_Publications_Roles]
	FOREIGN KEY([idRole]) REFERENCES Roles([idRole])

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
	FOREIGN KEY([idApplicant]) REFERENCES Users([idUser])
	   




INSERT INTO Roles(RoleName) VALUES('Recruiter'),('Applicant')

INSERT INTO Users(idRole, FirstName, LastName, Email, CreationDate)
VALUES(1,'Karla','Fuentes Nolasco','karla.fuente@fake.com',GETDATE()),
		(1,'Oscar','Kala Haak','oscar.kala@fake.com',GETDATE()),
		(1,'Maria','Torres','maria.torres@fake.com',GETDATE()),
		(2,'Xaime','Weir Rojo','xaime.rojo@fake.com',GETDATE()),
		(2,'Adriana','Fernandez','adriana.fernandez@fake.com',GETDATE()),
		(2,'Ingrid','Xodar Jimenez','ingrid.xodar@fake.com',GETDATE()),
		(2,'Berenice','Ximo Quezada','berenice.ximo@fake.com',GETDATE()),
		(2,'Rodrigo','Kitia Castro','rodrigo.kitia@fake.com',GETDATE()),
		(1,'Vladimir','Nikolaev Ivanov','vladimir.ivanov@fake.com',GETDATE()),
		(2,'Greta','Baumann Richter','greta.richter@fake.com',GETDATE()),
		(1,'Jessica','Johnson Castillo','jessica.johnson@fake.com',GETDATE()),
		(2,'Anastacia','Sokilova Braun','anastacia.braun@fake.com',GETDATE()),
		(1,'Michel','Johnsson Figueroa','michel.johnsson@fake.com',GETDATE()),
		(1,'Castro','Kennedy Schmitt','castro.kennedy@fake.com',GETDATE()),
		(2,'Jorge','Lopez Fernandez','jorge.lopez@fake.com',GETDATE()),
		(1,'Klaus','Schneider Wlaker','klaus.schneider@fake.com',GETDATE()),
		(1,'Sandra','Connor','sandra.connor@fake.com',GETDATE()),
		(2,'Clara','Luciani Le','clara.le@fake.com',GETDATE()),
		(1,'William','Wisher Jr','william.wisher@fake.com',GETDATE()),
		(1,'Franco','Columbu Bess','franco.columbu@fake.com',GETDATE()),
		(2,'Anna','Romanova Kuznetsova','anna.romanova@fake.com',GETDATE()),
		(1,'Fritz','Fisher Weber','fritz.fisher@fake.com',GETDATE()),
		(1,'Maria','Garcia Lopez','maria.garcia@fake.com',GETDATE()),
		(1,'Olga','Hernandez Ramirez','olga.hernandez@fake.com',GETDATE()),
		(2,'Antonio','Ramirez Acu a','antonio.ramirez@fake.com',GETDATE()),
		(1,'Emilio','Gonzales','emilio.gonzales@fake.com',GETDATE()),
		(2,'Sofia','Salazar Hernandez','sofia.salazar@fake.com',GETDATE()),
		(1,'Hans','Mueller Schmidt','hans.mueller@fake.com',GETDATE()),
		(2,'John','Cena Johnson','john.cena@fake.com',GETDATE()),
		(1,'Rosa','Delgadillo Paredes','rosa.delgadillo@fake.com',GETDATE()),
		(1,'Pedro','Aguilar Rojas','pedro.agular@fake.com',GETDATE()),
		(1,'Alejandro','Rivera Vargas','alejandro.rivera@fake.com',GETDATE()),
		(2,'Jose','Alfredo Jimenez','jose.alfredo@fake.com',GETDATE()),
		(1,'Elena','Mendoza Castro','elena.mendoza@fake.com',GETDATE()),
		(1,'Lucia','Rivera Garcia','lucia.rivera@fake.com',GETDATE()),
		(2,'Laura','Morales Dias','laura.dias@fake.com',GETDATE()),
		(1,'Wagner','Mozart Bach','wagner.mozart@fake.com',GETDATE()),
		(1,'Darwin','Walker','darwin.walker@fake.com',GETDATE()),
		(1,'Anna','petrova Ivanova','anna.petrova@fake.com',GETDATE()),
		(2,'Jessica','Kinolev Smith','jessica.kinolev@fake.com',GETDATE()),
		(1,'Ivano','Thompson Weber','ivano.thompson@fake.com',GETDATE()),
		(1,'Elena','Romero Morales','elena.romero@fake.com',GETDATE()),
		(2,'Juan','Hernandez Hernandez','juan.hernandez@fake.com',GETDATE()),
		(1,'Sofia','Silva','sofia.silva@fake.com',GETDATE())