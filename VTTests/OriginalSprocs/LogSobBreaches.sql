USE [VirtualTrainer]
GO

/****** Object:  StoredProcedure [dbo].[vt_live_LogSOBBreaches]    Script Date: 02/17/2017 17:09:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Scott Chittick>
-- Create date: <Create Date,,>
-- Description:	<Description,,Source of business missing procedure.>
-- =============================================
CREATE PROCEDURE [dbo].[vt_live_LogSOBBreaches]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	create table #temp(context_ref int, handler_key int, 
						broker_office_key int, broker_key int, client_name nvarchar(120),

)
	insert into #temp select c.client_key,  c.primary_account_handler_key, c.broker_office_key, c.broker_key,
						c.name
						from [GIB-DB01].Giles_BDW_History.acturis.client c	
						left join (select * from [GIB-DB01].Giles_BDW_History.acturis.contact_reprt_fields_up where contact_reprt_question_key = '604') rf --604 = Source of business.
						on rf.client_key = c.client_key
						join dbo.vt_live_conf_active_rules r on c.broker_office_key = r.office_key
						where (lead_date_key >= DATEADD(dd, -7, GETDATE())
						or
						c.client_key in (select distinct context_ref from dbo.vt_live_breach_log where rule_code = 'SOB')
						)
							-- excluded product setting	(product is policy level only)
							/*and p.broker_product_target_key not in (
								select product_key from dbo.vt_live_conf_excl_product
								where rule_code = 'SOB'
							)*/
							-- excluded agent setting
							and c.agent_key not in(
								select agent_key from dbo.vt_live_conf_excl_agent
								where rule_code = 'SOB'
							)
						
						--excluded teams settings
						and c.primary_account_handler_key not in (
							select member_key from dbo.vt_live_conf_team
							where member_type = 'h' and team_key in (
								select team_key from dbo.vt_live_conf_excl_team
								where rule_code = 'SOB'
							)
						)
						and c.broker_office_key not in (
							select member_key from dbo.vt_live_conf_team
							where member_type = 'o' and team_key in (
								select team_key from dbo.vt_live_conf_excl_team
								where rule_code = 'SOB'
							)
						)
						--active rules settings
						and r.sob = 1
						and c.broker_key = 991	-- this rule is only Giles brokerage specific
						and rf.client_key is NULL
						and c.client_status_key not in (2,10) --lapsed and closed
						
				
	declare @context int
	declare @handler int
	declare @office int
	declare @broker int
	declare @client nvarchar(120)
		
	WHILE EXISTS(SELECT TOP 1 * FROM #TEMP) 
		BEGIN 
			SELECT TOP 1 @context = context_ref, @handler = handler_key, @office = broker_office_key, @broker = broker_key, @client = client_name FROM #TEMP 

			/*** Do my Processing to add multiple records ***/ 
			INSERT INTO dbo.vt_live_breach_log (rule_code, context_ref, handler_key, office_key, team_key, broker_key, log_date) VALUES ('SOB', @context, @handler, @office, @office, @broker, getdate())
			IF (select count(*) from dbo.vt_live_sob_archive where context_ref = @context) = 0
				BEGIN
					insert into dbo.vt_live_sob_archive (context_ref, client_name) 
												values (@context, @client)
				END
			ELSE
				BEGIN
					update dbo.vt_live_breach_log set handler_key = @handler, office_key = @office where rule_code='SOB' and context_ref=@context
					update dbo.vt_live_sob_archive set client_name=@client  where context_ref=@context
				END
			DELETE TOP(1) from #TEMP
		END
	DROP TABLE #TEMP

END

GO

