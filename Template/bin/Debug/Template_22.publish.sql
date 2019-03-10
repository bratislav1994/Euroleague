﻿/*
Deployment script for Euroleague3

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Euroleague3"
:setvar DefaultFilePrefix "Euroleague3"
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
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                RECOVERY FULL 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET PAGE_VERIFY NONE 
            WITH ROLLBACK IMMEDIATE;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Dropping [dbo].[Igrac_Klub_FK]...';


GO
ALTER TABLE [dbo].[Igrac] DROP CONSTRAINT [Igrac_Klub_FK];


GO
PRINT N'Dropping [dbo].[IgracIgra_Igrac_FK]...';


GO
ALTER TABLE [dbo].[IgracIgra] DROP CONSTRAINT [IgracIgra_Igrac_FK];


GO
PRINT N'Dropping [dbo].[IgracIgra_Utakmica_FK]...';


GO
ALTER TABLE [dbo].[IgracIgra] DROP CONSTRAINT [IgracIgra_Utakmica_FK];


GO
PRINT N'Dropping [dbo].[Klub_Evroliga_FK]...';


GO
ALTER TABLE [dbo].[Klub] DROP CONSTRAINT [Klub_Evroliga_FK];


GO
PRINT N'Dropping [dbo].[Rezervacija_Hala_FK]...';


GO
ALTER TABLE [dbo].[Rezervacija] DROP CONSTRAINT [Rezervacija_Hala_FK];


GO
PRINT N'Dropping [dbo].[Rezervacija_Klub_FK]...';


GO
ALTER TABLE [dbo].[Rezervacija] DROP CONSTRAINT [Rezervacija_Klub_FK];


GO
PRINT N'Dropping [dbo].[Sudi_Sudija_FK]...';


GO
ALTER TABLE [dbo].[Sudi] DROP CONSTRAINT [Sudi_Sudija_FK];


GO
PRINT N'Dropping [dbo].[Sudi_Utakmica_FK]...';


GO
ALTER TABLE [dbo].[Sudi] DROP CONSTRAINT [Sudi_Utakmica_FK];


GO
PRINT N'Dropping [dbo].[Sudija_Evroliga_FK]...';


GO
ALTER TABLE [dbo].[Sudija] DROP CONSTRAINT [Sudija_Evroliga_FK];


GO
PRINT N'Dropping [dbo].[Trener_Klub_FK]...';


GO
ALTER TABLE [dbo].[Trener] DROP CONSTRAINT [Trener_Klub_FK];


GO
PRINT N'Dropping [dbo].[Utakmica_Evroliga_FK]...';


GO
ALTER TABLE [dbo].[Utakmica] DROP CONSTRAINT [Utakmica_Evroliga_FK];


GO
PRINT N'Dropping [dbo].[Utakmica_Klub_FK]...';


GO
ALTER TABLE [dbo].[Utakmica] DROP CONSTRAINT [Utakmica_Klub_FK];


GO
PRINT N'Dropping [dbo].[Utakmica_Klub_FKv2]...';


GO
ALTER TABLE [dbo].[Utakmica] DROP CONSTRAINT [Utakmica_Klub_FKv2];


GO
PRINT N'Creating [dbo].[DateValidationReservation]...';


GO
CREATE PROCEDURE DateValidationReservation
	(
	@startDate date,
	@endDate date,
	@hallId varchar(15),
	@isOk varchar(50) out
)
AS

BEGIN
	set @isOk = 'ok';
	declare @startDateFromDB date, @endDateFromDB date;
	declare @RezervacijaCursor as cursor;

	set @RezervacijaCursor = cursor for 
	select Hala_OZN_HALA, PVRMREZ_HALA, KVRMREZ_HALA
	from Rezervacija
	where Hala_OZN_HALA = @hallId;

	if ISDATE(cast(@startDate as varchar)) = 0 or ISDATE(cast(@endDate as varchar)) = 0
		begin
			set @isOk = 'Invalid date!';
			return;
		end
		

	if DATEDIFF(day, @startDate, @endDate) < 0
		begin
			set @isOk = 'Start date must be before end date!';
			return;
		end
       
	open @RezervacijaCursor;
	fetch next from @RezervacijaCursor into @hallId, @startDateFromDB, @endDateFromDB;

	while @@FETCH_STATUS = 0

	begin

		if DATEDIFF(day, @startDate, @startDateFromDB) = 0 or DATEDIFF(day, @endDate, @endDateFromDB) = 0 
			or DATEDIFF(day, @startDate, @endDateFromDB) = 0 or DATEDIFF(day, @endDate, @startDateFromDB) = 0
			begin
				set @isOk = 'Reservation in this period already exists!';
				return;
			end

		if DATEDIFF(day, @startDate, @startDateFromDB) > 0 -- if startDate is before startDateDB 
			begin
			if DATEDIFF(day, @endDate, @startDateFromDB) <= 0
				begin
					set @isOk = 'Reservation in this period already exists!';
					return;
				end
			end
		else
			if DATEDIFF(day, @startDate, @endDateFromDB) >= 0 or DATEDIFF(day, @endDate, @endDateFromDB) >= 0 
				begin
					set @isOk = 'Reservation in this period already exists!';
					return;
				end
		
		fetch next from @RezervacijaCursor into @hallId, @startDateFromDB, @endDateFromDB;
		
	end
 
	close @RezervacijaCursor;
	deallocate @RezervacijaCursor;

	END
GO
PRINT N'Creating [dbo].[DateValidationReservationModify]...';


GO
CREATE PROCEDURE DateValidationReservationModify
	(
	@startDate date,
	@endDate date,
	@oldRezId varchar(64),
	@oldHalaOzn varchar(64),
	@isOk varchar(50) out
)
AS

BEGIN
	set @isOk = 'ok';
	declare @startDateFromDB date, @endDateFromDB date;
	declare @rez_id varchar(64), @hala_ozn varchar(64);
	declare @RezervacijaCursor as cursor;

	set @RezervacijaCursor = cursor for 
	select Hala_OZN_HALA, PVRMREZ_HALA, KVRMREZ_HALA, SFR_REZ
	from Rezervacija
	where Hala_OZN_HALA = @oldHalaOzn;

	if ISDATE(cast(@startDate as varchar)) = 0 or ISDATE(cast(@endDate as varchar)) = 0
		begin
			set @isOk = 'Invalid date!';
			return;
		end
		

	if DATEDIFF(day, @startDate, @endDate) < 0
		begin
			set @isOk = 'Start date must be before end date!';
			return;
		end
       
	open @RezervacijaCursor;
	fetch next from @RezervacijaCursor into @hala_ozn, @startDateFromDB, @endDateFromDB, @rez_id;

	while @@FETCH_STATUS = 0

	begin
		if @oldRezId != @rez_id
		begin
		
		if DATEDIFF(day, @startDate, @startDateFromDB) = 0 or DATEDIFF(day, @endDate, @endDateFromDB) = 0 
			or DATEDIFF(day, @startDate, @endDateFromDB) = 0 or DATEDIFF(day, @endDate, @startDateFromDB) = 0
			begin
				set @isOk = 'Reservation in this period already exists!';
				return;
			end

		if DATEDIFF(day, @startDate, @startDateFromDB) > 0 -- if startDate is before startDateDB 
			begin
			if DATEDIFF(day, @endDate, @startDateFromDB) <= 0
				begin
					set @isOk = 'Reservation in this period already exists!';
					return;
				end
			end
		else
			if DATEDIFF(day, @startDate, @endDateFromDB) >= 0 or DATEDIFF(day, @endDate, @endDateFromDB) >= 0 
				begin
					set @isOk = 'Reservation in this period already exists!';
					return;
				end
		end

		fetch next from @RezervacijaCursor into @hala_ozn, @startDateFromDB, @endDateFromDB, @rez_id;
		
	end
 
	close @RezervacijaCursor;
	deallocate @RezervacijaCursor;

	END
GO
PRINT N'Creating [dbo].[Stats]...';


GO
CREATE PROCEDURE Stats
	(
	@playerId varchar(64), 
	@name varchar(64) out, 
	@games int out, 
	@pts float out, 
	@as float out, 
	@reb float out
)
AS

BEGIN
	declare @stsPod varchar(64);
	declare @sumPts int, @sumAs int, @sumReb int;
	declare @getPts int, @getAs int, @getReb int;
	declare @PlayerCursor as cursor;
	
	select @name = IME_IGR + ' ' + PRZ_IGR
	FROM Igrac
	WHERE LICBR_IGR = @playerId;

	set @PlayerCursor = cursor for 
	select POENI_IGRACIGRA, AS_IGRACIGRA, SK_IGRACIGRA
	from IgracIgra
	where @playerId = Igrac_LICBR_IGR;
       
	open @PlayerCursor;
	fetch next from @PlayerCursor into @getPts, @getAs, @getReb;

	set @games = 0;
	set @sumAs = 0;
	set @sumPts = 0;
	set @sumReb = 0;

	while @@FETCH_STATUS = 0

	begin

		set @sumPts = @sumPts + @getPts;
		set @sumAs = @sumAs + @getAs;
		set @sumReb = @sumReb + @getReb;
		set @games = @games + 1;
		
		fetch next from @PlayerCursor into @getPts, @getAs, @getReb;
		
	end

	if @games != 0
		begin
			set @pts = cast(@sumPts as float)/cast(@games as float);
			set @as = cast(@sumAs as float)/cast(@games as float);
			set @reb = cast(@sumReb as float)/cast(@games as float);
		end
 
	close @PlayerCursor;
	deallocate @PlayerCursor;

END
GO
PRINT N'Update complete.';


GO