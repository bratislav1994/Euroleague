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