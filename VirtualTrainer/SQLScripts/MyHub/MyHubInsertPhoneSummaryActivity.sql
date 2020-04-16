CREATE PROCEDURE [dbo].[MyHubInsertPhoneSummaryActivity] 
	-- Add the parameters for the stored procedure here
	@VTBreaches VTBreachType READONLY	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Fixed line rental charge for each user.
	DECLARE @lineRental Int = 960;
	
	insert into [PhoneSummaryActivities] 
	(
	   [PhoneNumber]
	  ,[UserName]	  
	  ,[EmployeeID]
	  ,[CostCentreID]
	  ,[NumberOfCalls]
	  ,[TotalDuration]
	  ,[DataVolumeKB]
	  ,[Month]
	  ,[Year]
	  ,[ReportDate]
	  ,[LineRentalInc]
	  ,[TotalCost]
	  ,[TotalUsageChargesInc]
	)
	select	
		b.RuleBreachFieldOne -- phone no
		,b.RuleBreachFieldTwo -- user name
		,b.RuleBreachFieldThree -- EmployeeId
		,b.RuleBreachFieldFour -- cost centre
		,REPLACE(b.RuleBreachFieldFive,',','') -- no of calls
		,(Substring(b.RuleBreachFieldSix,1,2)* 60 * 60) + (Substring(b.RuleBreachFieldSix,4,2)* 60) + (Substring(b.RuleBreachFieldSix,7,2)) -- total duration - like hh:mm:ss
		,REPLACE(b.RuleBreachFieldSeven,',','') -- volume kb	
		,b.ContextRef -- month
		,b.ContextRef -- year
		,CAST(CAST(Year(b.RuleBreachFieldThirteen) as varchar) + '-' + CAST(Day(b.RuleBreachFieldThirteen) as varchar) + '-' + '1' as DateTime) -- ReportDate - use Day because of original format actally year and Month needed
		,@lineRental -- line rental
		,((CAST(REPLACE(REPLACE(REPLACE(b.RuleBreachFieldTwelve,'£',''),'.',''),',','') as Int) 
			- (CAST(REPLACE(REPLACE(REPLACE(b.RuleBreachFieldEight,'£',''),'.',''),',','') as Int) * 1.2)
		  ) + @lineRental)-- total cost
		, ((CAST(REPLACE(REPLACE(REPLACE(b.RuleBreachFieldTwelve,'£',''),'.',''),',','') as Int) 
			- (CAST(REPLACE(REPLACE(REPLACE(b.RuleBreachFieldEight,'£',''),'.',''),',','') as Int) * 1.2)
		  ) + @lineRental) - @lineRental -- Usage Cost
	from @VTBreaches as b
	Where b.RuleBreachFieldTwo is not null
	and b.RuleBreachFieldTwo <> ''
	and b.RuleBreachFieldTwo <> 'User Name'
	and NOT EXISTS
	(
	   SELECT Null
	   FROM [PhoneSummaryActivities] p 
	   WHERE CAST(CAST(Year(b.RuleBreachFieldThirteen) as varchar) + '-' + CAST(Day(b.RuleBreachFieldThirteen) as varchar) + '-' + '1' as DateTime) = p.[ReportDate] 	   
	   AND b.RuleBreachFieldThree = p.[EmployeeID] 
	   AND b.RuleBreachFieldOne = p.[PhoneNumber]
	);	
	
	
END