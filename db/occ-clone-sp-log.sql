USE [OCC_CLONE]
GO

IF OBJECT_ID('[dbo].[Usp_IntegrationEvents_Add]','P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[Usp_IntegrationEvents_Add]
END
GO

CREATE OR ALTER PROCEDURE 
	[dbo].[Usp_IntegrationEvents_Add]
		@IdCorrelation NVARCHAR(MAX),
		@EventType NVARCHAR(100),
		@EventData NVARCHAR(MAX),
		@Exchange NVARCHAR(100),
		@RoutingKey NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
			
		INSERT INTO IntegrationEvents(IdCorrelation, EventType, EventData, Exchange, RoutingKey, CreatedAt)
		VALUES(@IdCorrelation, @EventType, @EventData, @Exchange, @RoutingKey, GETDATE())

	SET NOCOUNT OFF;
END
GO
