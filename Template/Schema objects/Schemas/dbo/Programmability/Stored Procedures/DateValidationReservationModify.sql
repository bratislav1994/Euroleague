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