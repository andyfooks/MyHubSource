USE [VirtualTrainer]
GO

/****** Object:  StoredProcedure [dbo].[vt_live_LogINTBreaches]    Script Date: 02/17/2017 17:09:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Ales Remta>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[vt_live_LogINTBreaches]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	create table #temp(context_ref nvarchar(10), handler_key int, 
						broker_office_key int, broker_key int, product_target nchar(100),
						version_ref int, client_key int)
						
	insert into #temp select distinct p.policy_ref, p.policy_account_handler_key,
					p.policy_broker_office_key, p.policy_broker_key, 
					t.product_target_name, p.version_ref, p.client_key
					from Giles_BDW_History.acturis.policy p
					inner join Giles_BDW_History.acturis.broker_product_target t on (p.broker_product_target_key = t.broker_product_target_key)
					inner join VirtualTrainer.dbo.vt_live_conf_active_rules r on p.policy_broker_office_key = r.office_key
					where (p.policy_status_key = 7 or p.policy_status_key = 8)
					and p.cancellation_date_key is NULL
					and p.accepted_date_key <= DATEADD(dd,-7,GETDATE())
					and p.version_ref = p.last_in_band_version_ref
					and ((p.business_event_key = 2 and p.total_insurer_amount > 0) or p.business_event_key <> 2)
					and p.business_event_key <> 9
					and p.primary_introducer_key = -1
					and p.policy_broker_key = 4378
					-- active rules setting
					and r.int = 1
					-- excluded team setting
						and p.policy_account_handler_key not in (
							select member_key from VirtualTrainer.dbo.vt_live_conf_team
							where member_type = 'h' and team_key in (
								select team_key from VirtualTrainer.dbo.vt_live_conf_excl_team
								where rule_code = 'INT'
							)
						)
						and p.policy_broker_office_key not in (
							select member_key from VirtualTrainer.dbo.vt_live_conf_team
							where member_type = 'o' and team_key in (
								select team_key from VirtualTrainer.dbo.vt_live_conf_excl_team
								where rule_code = 'INT'
							)
						)
						-- excluded product setting
						and p.broker_product_target_key not in (
							select product_key from VirtualTrainer.dbo.vt_live_conf_excl_product
							where rule_code = 'INT'
						)
						-- excluded agents setting
						and p.policy_agent_key not in(
							select agent_key from VirtualTrainer.dbo.vt_live_conf_excl_agent
							where rule_code = 'INT'
						)

	declare @context int
	declare @handler int
	declare @office int
	declare @broker int
	declare @client int
	declare @version int
	declare @product nchar(100)
	
	WHILE EXISTS(SELECT TOP 1 * FROM #TEMP) 
		BEGIN 
			SELECT TOP 1 @context = context_ref, @handler = handler_key, @office = broker_office_key, @broker = broker_key, @client = client_key, @product = product_target, @version = version_ref FROM #TEMP 

			/*** Do my Processing to add multiple records ***/ 
			INSERT INTO dbo.vt_live_breach_log (rule_code, context_ref, handler_key, office_key, team_key, broker_key, log_date) VALUES ('INT', @context, @handler, @office, @office, @broker, getdate())
			IF (select count(*) from dbo.vt_live_int_archive where context_ref = @context) = 0
			BEGIN
				--insert into dbo.vt_live_archive (rule_code, context_ref, handler_key, office_key, log_date, log_count) values ('PNO', @context, @handler, @office, getdate(), 1)
				insert into dbo.vt_live_int_archive (context_ref, version_ref, client_key, client_name, product_target) 
											values (@context, @version, @client, dbo.GetClientName(@client), @product)
			END
			ELSE
			BEGIN
				update dbo.vt_live_breach_log set handler_key = @handler, office_key = @office where rule_code='INT' and context_ref=@context
				update dbo.vt_live_int_archive set client_name=dbo.getclientname(@client), version_ref = @version, product_target = @product where context_ref=@context
			END
			DELETE TOP(1) from #TEMP
		END
	DROP TABLE #TEMP

END

GO

