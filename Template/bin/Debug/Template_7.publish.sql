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
PRINT N'Creating [dbo].[Stats]...';


GO
CREATE PROCEDURE Stats
	(
	@playerId varchar(15), 
	@name varchar(30) out, 
	@games int out, 
	@pts float out, 
	@as float out, 
	@reb float out
)
AS

BEGIN
	declare @stsPod varchar(30);
	declare @sumPts int, @sumAs int, @sumReb int;
	declare @getPts int, @getAs int, @getReb int;
	declare @PlayerCursor as cursor;

	select @name = IME_IGR + ' ' + PRZ_IGR
	FROM Igrac
	WHERE LICBR_IGR = @playerId;

	set @PlayerCursor = cursor for 
	select StatistickiPodaci_OZN_STSPOD
	from IgracIgra
	where @playerId = Igrac_LICBR_IGR;
       
	open @PlayerCursor;
	fetch next from @PlayerCursor into @stsPod;

	set @games = 0;
	set @sumAs = 0;
	set @sumPts = 0;
	set @sumReb = 0;

	while @@FETCH_STATUS = 0

	begin

		select @getPts = PTS_STSPOD, @getAs = ASS_STSPOD, @getReb = SK_STSPOD 
		from @stsPod;

		set @sumPts = @sumPts + @getPts;
		set @sumAs = @sumAs + @getAs;
		set @sumReb = @sumReb + @getReb;
		set @games = @games + 1;
		
		fetch next from @PlayerCursor into @stsPod;
		
	end

	if games != 0
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
