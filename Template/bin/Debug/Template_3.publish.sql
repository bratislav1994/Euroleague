﻿/*
Deployment script for Euroleague

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Euroleague"
:setvar DefaultFilePrefix "Euroleague"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
/*
The column [dbo].[Sudi].[Id_SUD] on table [dbo].[Sudi] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/

IF EXISTS (select top 1 1 from [dbo].[Sudi])
    RAISERROR (N'Rows were detected. The schema update is terminating because data loss might occur.', 16, 127) WITH NOWAIT

GO
PRINT N'Dropping [dbo].[Sudi_Sudija_FK]...';


GO
ALTER TABLE [dbo].[Sudi] DROP CONSTRAINT [Sudi_Sudija_FK];


GO
PRINT N'Dropping [dbo].[Sudi_Utakmica_FK]...';


GO
ALTER TABLE [dbo].[Sudi] DROP CONSTRAINT [Sudi_Utakmica_FK];


GO
PRINT N'Starting rebuilding table [dbo].[Sudi]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Sudi] (
    [Id_SUD]           NVARCHAR (20) NOT NULL,
    [Sudija_LICBR_SUD] NVARCHAR (20) NOT NULL,
    [Utakmica_OZN_UTK] NVARCHAR (20) NOT NULL,
    [Utakmica_OZN_LIG] NVARCHAR (20) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_Sudi_PK1] PRIMARY KEY CLUSTERED ([Id_SUD] ASC, [Sudija_LICBR_SUD] ASC, [Utakmica_OZN_UTK] ASC, [Utakmica_OZN_LIG] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Sudi])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Sudi] ([Sudija_LICBR_SUD], [Utakmica_OZN_UTK], [Utakmica_OZN_LIG])
        SELECT   [Sudija_LICBR_SUD],
                 [Utakmica_OZN_UTK],
                 [Utakmica_OZN_LIG]
        FROM     [dbo].[Sudi]
        ORDER BY [Sudija_LICBR_SUD] ASC, [Utakmica_OZN_UTK] ASC, [Utakmica_OZN_LIG] ASC;
    END

DROP TABLE [dbo].[Sudi];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Sudi]', N'Sudi';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_Sudi_PK1]', N'Sudi_PK', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Update complete.';


GO
