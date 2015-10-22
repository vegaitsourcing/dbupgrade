IF EXISTS (SELECT name 
	   FROM   sysobjects 
	   WHERE  name = 'GetAllCompanies'
	   AND 	  type = 'P')
    DROP PROCEDURE dbo.GetAllCompanies
GO

CREATE  PROCEDURE dbo.GetAllCompanies
AS
	SELECT * FROM dbo.Companies