USE [master]
GO

CREATE DATABASE [SpeedUpCoreAPIExampleDB] 
GO 
 
USE [SpeedUpCoreAPIExampleDB] 
GO 

CREATE TABLE [dbo].[Products] (
    [ProductId] INT         IDENTITY (1, 1) NOT NULL,
    [SKU]       NCHAR (50)  NOT NULL,
    [Name]      NCHAR (150) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductId] ASC)
);
GO

CREATE TABLE [dbo].[Prices] (
    [PriceId]   INT             IDENTITY (1, 1) NOT NULL,
    [ProductId] INT             NOT NULL,
    [Value]     DECIMAL (18, 2) NOT NULL,
    [Supplier]  NCHAR (50)      NOT NULL,
    CONSTRAINT [PK_Prices] PRIMARY KEY CLUSTERED ([PriceId] ASC)
);
GO

ALTER TABLE [dbo].[Prices]  WITH CHECK ADD  CONSTRAINT [FK_Prices_Products] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Prices] CHECK CONSTRAINT [FK_Prices_Products]
GO

INSERT INTO Products ([SKU], [Name]) VALUES ('aaa', 'Product1');
INSERT INTO Products ([SKU], [Name]) VALUES ('aab', 'Product2');
INSERT INTO Products ([SKU], [Name]) VALUES ('abc', 'Product3');

INSERT INTO Prices ([ProductId], [Value], [Supplier]) VALUES (1, 100, 'Bosch');
INSERT INTO Prices ([ProductId], [Value], [Supplier]) VALUES (1, 125, 'LG');
INSERT INTO Prices ([ProductId], [Value], [Supplier]) VALUES (1, 130, 'Garmin');

INSERT INTO Prices ([ProductId], [Value], [Supplier]) VALUES (2, 140, 'Bosch');
INSERT INTO Prices ([ProductId], [Value], [Supplier]) VALUES (2, 145, 'LG');
INSERT INTO Prices ([ProductId], [Value], [Supplier]) VALUES (2, 150, 'Garmin');

INSERT INTO Prices ([ProductId], [Value], [Supplier]) VALUES (3, 160, 'Bosch');
INSERT INTO Prices ([ProductId], [Value], [Supplier]) VALUES (3, 165, 'LG');
INSERT INTO Prices ([ProductId], [Value], [Supplier]) VALUES (3, 170, 'Garmin');

GO

USE [SpeedUpCoreAPIExampleDB] 
GO 

ALTER TABLE [Products]
ALTER COLUMN SKU nvarchar(50) NOT NULL

ALTER TABLE [Products]
ALTER COLUMN [Name] nvarchar(150) NOT NULL

ALTER TABLE [Prices]
ALTER COLUMN Supplier nvarchar(50) NOT NULL

USE [SpeedUpCoreAPIExampleDB] 
GO 

UPDATE Products SET SKU = RTRIM(SKU), Name = RTRIM(Name) 
GO

UPDATE Prices SET  Supplier = RTRIM(Supplier)
GO

USE [SpeedUpCoreAPIExampleDB] 
GO 

CREATE FULLTEXT CATALOG [ProductsFTS] WITH ACCENT_SENSITIVITY = ON
AS DEFAULT
GO

USE [SpeedUpCoreAPIExampleDB] 
GO 

CREATE FULLTEXT INDEX ON [dbo].[Products]
(SKU LANGUAGE 1033)
KEY INDEX PK_Products
ON ProductsFTS
GO

USE [SpeedUpCoreAPIExampleDB]
GO

CREATE PROCEDURE [dbo].[GetProductsBySKU]
	@sku [varchar] (50) 
AS
BEGIN
	SET NOCOUNT ON;

	Select @sku = '"' + @sku + '*"'

    -- Insert statements for procedure here
	SELECT ProductId, SKU, Name FROM [dbo].Products WHERE CONTAINS(SKU, @sku)
END
GO
USE [SpeedUpCoreAPIExampleDB]
GO

EXEC [dbo].[GetProductsBySKU] 'aa'
GO

USE [SpeedUpCoreAPIExampleDB]
GO

--clear cache
DBCC FREEPROCCACHE

SELECT cplan.usecounts, cplan.objtype, qtext.text, qplan.query_plan
FROM sys.dm_exec_cached_plans AS cplan
CROSS APPLY sys.dm_exec_sql_text(plan_handle) AS qtext
CROSS APPLY sys.dm_exec_query_plan(plan_handle) AS qplan
ORDER BY cplan.usecounts DESC


USE [SpeedUpCoreAPIExampleDB] 
GO 

ALTER TABLE [Prices]
ADD xProductId AS convert(nvarchar(10), ProductId) PERSISTED NOT NULL
GO 

USE [SpeedUpCoreAPIExampleDB] 
GO 

CREATE FULLTEXT CATALOG [PricesFTS] WITH ACCENT_SENSITIVITY = ON
AS DEFAULT
GO

CREATE FULLTEXT INDEX ON [dbo].[Prices]
(xProductId LANGUAGE 1033)
KEY INDEX PK_Prices
ON PricesFTS
GO

USE [SpeedUpCoreAPIExampleDB]
GO

CREATE PROCEDURE [dbo].[GetPricesByProductId]
	@productId [int]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @xProductId [NVARCHAR] (10)
	Select @xProductId = '"' + CONVERT([nvarchar](10),@productId) + '"'

    -- Insert statements for procedure here
	SELECT PriceId, ProductId, [Value], Supplier FROM [dbo].Prices WHERE CONTAINS(xProductId, @xProductId)
END
GO

USE [SpeedUpCoreAPIExampleDB]
GO

DECLARE	@return_value int
EXEC	@return_value = [dbo].[GetPricesByProductId]
		@productId = 1

SELECT	'Return Value' = @return_value
GO

SELECT display_term FROM sys.dm_fts_parser (' "1" ', 1033, 0, 0)
SELECT display_term FROM sys.dm_fts_parser (' "x1" ', 1033, 0, 0)

USE [SpeedUpCoreAPIExampleDB]
GO

DROP FULLTEXT INDEX ON [Prices]
GO

USE [SpeedUpCoreAPIExampleDB]
GO

ALTER TABLE [Prices]
DROP COLUMN xProductId
GO

USE [SpeedUpCoreAPIExampleDB] 
GO 

ALTER TABLE [Prices]
ADD xProductId AS 'x' + convert(nvarchar(10), ProductId) PERSISTED NOT NULL
GO

USE [SpeedUpCoreAPIExampleDB] 
GO 

SELECT * FROM [Prices]
GO

USE [SpeedUpCoreAPIExampleDB] 
GO 

CREATE FULLTEXT INDEX ON [dbo].[Prices]
(xProductId LANGUAGE 1033)
KEY INDEX PK_Prices
ON PricesFTS
GO

USE [SpeedUpCoreAPIExampleDB]
GO

ALTER PROCEDURE [dbo].[GetPricesByProductId]
	@productId [int]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @xProductId [NVARCHAR] (10)
	Select @xProductId = '"x' + CONVERT([nvarchar](10),@productId) + '"'

    -- Insert statements for procedure here
	SELECT PriceId, ProductId, [Value], Supplier FROM [dbo].Prices WHERE CONTAINS(xProductId, @xProductId)
END

GO

USE [SpeedUpCoreAPIExampleDB]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[GetPricesByProductId]
		@productId = 1

SELECT	'Return Value' = @return_value

GO
