CREATE PROCEDURE [dbo].[MyHubInsertPrinterSummaryActivity] 
	-- Add the parameters for the stored procedure here
	@VTBreaches VTBreachType READONLY	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
    -- Insert statements for procedure here
    insert into [PrinterSummaryActivities] 
    (
		AccountID, 
		AccountIDBillable,
		AccountName,
		AccountDescription,	
		PrinterName,
		PrinterDescription,
		PrinterID,
		Department,
		TotalPages,
		BWPages,
		ColourPages,
		Amount,
		AltCost,
		Jobs,
		Billable,
		[Month],
		[Year],
		ReportDate
	)
    select	b.RuleBreachFieldOne
			,b.RuleBreachFieldTwo
			,b.ContextRef
			,b.RuleBreachFieldFour
			,b.RuleBreachFieldFive
			,b.RuleBreachFieldSix
			,b.RuleBreachFieldSeven
			,b.RuleBreachFieldEight
			,b.RuleBreachFieldNine
			,b.RuleBreachFieldTen
			,b.RuleBreachFieldEleven
			,b.RuleBreachFieldTwelve
			,b.RuleBreachFieldThirteen
			,b.RuleBreachFieldFourteen
			,b.RuleBreachFieldFifteen
			,Substring(b.BreachDisplayText,0,4)
			,Substring(b.BreachDisplayText,4,4)
			,CAST(Substring(b.BreachDisplayText,4,4) + '-' +  Substring(b.BreachDisplayText,0,4) + '-' + '1' as DateTime)
	from @VTBreaches as b
	Where NOT EXISTS
	(
       SELECT NULL 
       FROM [PrinterSummaryActivities] p 
       WHERE Substring(b.[BreachDisplayText],0,4) = p.[Month]
       and Substring(b.[BreachDisplayText],4,4) = p.[Year]
       AND b.[ContextRef] = p.[AccountName] 
       AND b.[RuleBreachFieldFive] = p.[PrinterName]
	);
	
END