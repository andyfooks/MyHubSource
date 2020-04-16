USE [VirtualTrainer]
GO

/****** Object:  StoredProcedure [dbo].[vt_live_LogUINBreaches]    Script Date: 02/17/2017 17:10:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Ales Remta>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[vt_live_LogUINBreaches]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	create table #temp(context_ref int, handler_key int, 
						broker_office_key int, broker_key int, client_key int,
						client_name nvarchar(150), version_ref int, product_target nvarchar(150),
						primary_insurer nvarchar(150))
				insert into #temp select p.policy_ref, p.policy_account_handler_key, p.policy_broker_office_key, 
						p.policy_broker_key, p.client_key, c.name, p.version_ref, t.product_target_name,
						i.insurer_name as broker_insurer
						from Giles_BDW_History.acturis.policy p
						join Giles_BDW_History.acturis.broker_insurer i on p.broker_insurer_key = i.broker_insurer_key
						join Giles_BDW_History.acturis.client c on c.client_key = p.client_key
						join Giles_BDW_History.acturis.broker_product_target t on p.broker_product_target_key = t.broker_product_target_key
						join VirtualTrainer.dbo.vt_live_conf_active_rules r on p.policy_broker_office_key = r.office_key
						left join Giles_BDW_History.acturis.underlying_insurer_bridge b on b.policy_key = p.policy_key
						left join Giles_BDW_History.acturis.underlying_insurer u on b.underlying_insurer_key = u.underlying_insurer_key
						where (p.policy_status_key = 8 or p.policy_status_key = 7)
						and p.cancellation_date_key is null
						and (p.accepted_date_key >= dateadd(dd, -7, getdate()) or p.policy_ref in (select context_ref from VirtualTrainer.dbo.vt_live_breach_log where rule_code = 'UIN'))
						and i.insurer_name in (select insurer_name from VirtualTrainer.dbo.vt_live_conf_insurer)
						and u.insurer_name is null
						and p.business_event_key <> 9 --no fees
						and p.version_ref = p.last_in_band_version_ref
						-- active rules setting
						and r.uin = 1
						-- excluded team setting
						and p.policy_account_handler_key not in (
							select member_key from VirtualTrainer.dbo.vt_live_conf_team
							where member_type = 'h' and team_key in (
								select team_key from VirtualTrainer.dbo.vt_live_conf_excl_team
								where rule_code = 'UIN'
							)
						)
						and p.policy_broker_office_key not in (
							select member_key from VirtualTrainer.dbo.vt_live_conf_team
							where member_type = 'o' and team_key in (
								select team_key from VirtualTrainer.dbo.vt_live_conf_excl_team
								where rule_code = 'UIN'
							)
						)
						-- excluded product setting
						and p.broker_product_target_key not in (
							select product_key from VirtualTrainer.dbo.vt_live_conf_excl_product
							where rule_code = 'UIN'
						)
						-- excluded agent setting
						and p.policy_agent_key not in(
							select agent_key from VirtualTrainer.dbo.vt_live_conf_excl_agent
							where rule_code = 'UIN'
						)
						order by p.policy_ref, p.version_ref	--order so the most recent is always in the uin archive
	
	declare @context int
	declare @handler int
	declare @office int
	declare @broker int
	declare @client int
	declare @name nvarchar(150)
	declare @product nvarchar(150)
	declare @insurer nvarchar(150)
	declare @version int
	
	WHILE EXISTS(SELECT TOP 1 * FROM #TEMP) 
		BEGIN 
			SELECT TOP 1 @context = context_ref, @handler = handler_key, @office = broker_office_key, @broker = broker_key, @client = client_key, @name = client_name, @version = version_ref, @product = product_target, @insurer = primary_insurer FROM #TEMP 

			/*** Do my Processing to add multiple records ***/ 
			IF ((select count(*) from dbo.vt_live_breach_log where context_ref = @context and rule_code = 'UIN' and log_date > dateadd(dd, -1, getdate())) = 0)
			BEGIN
				INSERT INTO dbo.vt_live_breach_log (rule_code, context_ref, handler_key, office_key, team_key, broker_key, log_date) VALUES ('UIN', @context, @handler, @office, @office, @broker, getdate())
			END
			IF (select count(*) from dbo.vt_live_uin_archive where context_ref = @context) = 0
				BEGIN
					insert into dbo.vt_live_uin_archive (context_ref, version_ref, client_key, client_name, product_target, primary_insurer) 
												values (@context, @version, @client, @name, @product, @insurer)
				END
			ELSE
				BEGIN
					update dbo.vt_live_breach_log set handler_key = @handler, office_key = @office where rule_code='UIN' and context_ref=@context
					update dbo.vt_live_uin_archive set primary_insurer=@insurer, client_name=@name, version_ref = @version where context_ref=@context
				END
			DELETE TOP(1) from #TEMP
		END
	DROP TABLE #TEMP

END

GO

