USE [VirtualTrainer]
GO

/****** Object:  StoredProcedure [dbo].[vt_live_LogTMPBreaches]    Script Date: 02/17/2017 17:09:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Ales Remta>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[vt_live_LogTMPBreaches]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	create table #temp(context_ref nvarchar(10), handler_key int, 
						broker_office_key int, broker_key int, primary_contact_key int,
						salutation nvarchar(100), home_phone nvarchar(60),
						work_phone nvarchar(60), mobile nvarchar(60), dob_date varchar(20))
	insert into #temp select distinct c.client_key, c.primary_account_handler_key, c.broker_office_key, c.broker_key, 
						pc.primary_contact_key, pc.salutation, pc.work_phone, pc.home_phone, pc.mobile, pc.dob_date 
						from Giles_BDW_History.acturis.client c
						inner join VirtualTrainer.dbo.vt_live_conf_active_rules r on c.broker_office_key = r.office_key
						inner join Giles_BDW_History.acturis.primary_contact pc on pc.primary_contact_key = c.primary_contact_key
						where c.client_key in (
							select distinct p.client_key from Giles_BDW_History.acturis.policy p
							where (p.policy_status_key = 7 or p.policy_status_key = 8)
							and p.cancellation_date_key is null
							and p.business_event_key <> 9 --no fees
							and 
(
	(
		p.accepted_date_key >= DATEADD(dd,-37,GETDATE()) 
		and p.accepted_date_key < DATEADD(dd, -7, GETDATE()) 
		and (pc.salutation is null or (pc.home_phone is null and pc.work_phone is null and pc.mobile is null))
	) 
	or 
	(

		(
		pc.salutation is null 
		or (pc.home_phone is null and pc.work_phone is null and pc.mobile is null)	
		or (pc.dob_date is null and (pc.position is null or pc.position = '') and c.company_type_key <> 11)
		)
		and p.client_key in (select distinct context_ref from VirtualTrainer.dbo.vt_live_breach_log where rule_code = 'TMP')
	)
)
							-- excluded product setting
							and p.broker_product_target_key not in (
								select product_key from VirtualTrainer.dbo.vt_live_conf_excl_product
								where rule_code = 'TMP'
							)
							-- excluded agent setting
							and p.policy_agent_key not in(
								select agent_key from VirtualTrainer.dbo.vt_live_conf_excl_agent
								where rule_code = 'TMP'
							)
						)
						--excluded teams settings
						and c.primary_account_handler_key not in (
							select member_key from VirtualTrainer.dbo.vt_live_conf_team
							where member_type = 'h' and team_key in (
								select team_key from VirtualTrainer.dbo.vt_live_conf_excl_team
								where rule_code = 'TMP'
							)
						)
						and c.broker_office_key not in (
							select member_key from VirtualTrainer.dbo.vt_live_conf_team
							where member_type = 'o' and team_key in (
								select team_key from VirtualTrainer.dbo.vt_live_conf_excl_team
								where rule_code = 'TMP'
							)
						)
						--active rules settings
						and r.tmp = 1
						

	declare @context nvarchar(10)
	declare @handler int
	declare @office int
	declare @broker int
	declare @contact int
	declare @salut nvarchar(100)
	declare @home nvarchar(60)
	declare @work nvarchar(60)
	declare @mob nvarchar(60)
	declare @dob nvarchar(20)
	
	WHILE EXISTS(SELECT TOP 1 * FROM #TEMP) 
		BEGIN 
			SELECT TOP 1 @context = context_ref, @handler = handler_key, @office = broker_office_key, @broker = broker_key, @contact = primary_contact_key, @salut = salutation, @home = home_phone, @work = work_phone, @mob = mobile, @dob = dob_date FROM #TEMP 

			/*** Do my Processing to add multiple records ***/ 
			INSERT INTO dbo.vt_live_breach_log (rule_code, context_ref, handler_key, office_key, team_key, broker_key, log_date) VALUES ('TMP', @context, @handler, @office, @office, @broker, getdate())
			IF (select count(*) from dbo.vt_live_tmp_archive where context_ref = @context) = 0
				BEGIN
					--insert into dbo.vt_live_archive (rule_code, context_ref, handler_key, office_key, log_date, log_count) values ('PNO', @context, @handler, @office, getdate(), 1)
					insert into dbo.vt_live_tmp_archive (context_ref, primary_contact_key, client_name, salutation, home_phone, work_phone, mobile, dob_date) 
												values (@context, @contact, dbo.GetClientName(@context), @salut, @home, @work, @mob, @dob)
				END
			ELSE
				BEGIN
					update dbo.vt_live_breach_log set handler_key = @handler, office_key = @office where rule_code='TMP' and context_ref=@context
					update dbo.vt_live_tmp_archive set primary_contact_key=@contact, client_name=dbo.GetClientName(@context), salutation=@salut, home_phone=@home, work_phone=@work, mobile=@mob, dob_date = @dob where context_ref=@context
				END
			DELETE TOP(1) from #TEMP
		END
	DROP TABLE #TEMP

END

GO

