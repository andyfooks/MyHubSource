USE [VirtualTrainer]
GO

/****** Object:  StoredProcedure [dbo].[vt_live_LogBORBreaches]    Script Date: 02/17/2017 17:08:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ales Remta
-- Create date: 06/2011
-- Description:	
-- Gets all cases from the bor_process_data table which the script
-- has identified as breaches and logs them in the breach_log as usual.
-- =============================================
CREATE PROCEDURE [dbo].[vt_live_LogBORBreaches]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	create table #temp(policy_ref int, version_ref int, handler_key int, 
						broker_office_key int, broker_key int, client_key int,
						client_name nvarchar(max), insurer_name nvarchar(max),
						product_target_name nvarchar(max), accepted_date datetime)
				
	insert into #temp select policy_ref, version_ref, handler_key, 
						p.office_key, p.broker_key, client_key, 
						client_name, insurer_name, product_target_name,
						accepted_date
						from VirtualTrainer.dbo.vt_live_bor_process_data p
						join VirtualTrainer.dbo.vt_live_conf_active_rules r
						on p.office_key = r.office_key 
						where flag = 'n'
						--excluded product setting
						and p.product_target_key not in (
							select product_key from VirtualTrainer.dbo.vt_live_conf_excl_product
							where rule_code = 'BOR'
						)
						-- active rule setting
						and r.bor = 1
						-- excluded team setting
						and p.handler_key not in (
							select member_key from VirtualTrainer.dbo.vt_live_conf_team
							where member_type = 'h' and team_key in (
								select team_key from VirtualTrainer.dbo.vt_live_conf_excl_team
								where rule_code = 'BOR'
							)
						)
						and p.office_key not in (
							select member_key from VirtualTrainer.dbo.vt_live_conf_team
							where member_type = 'o' and team_key in (
								select team_key from VirtualTrainer.dbo.vt_live_conf_excl_team
								where rule_code = 'BOR'
							)
						)
						--excluded agents
						and p.agent_key not in(
							select agent_key from VirtualTrainer.dbo.vt_live_conf_excl_agent
							where rule_code = 'BOR'
						)

	declare @policy int
	declare @version int
	declare @handler int
	declare @office int
	declare @broker int
	declare @client int
	declare @c_name nvarchar(max)
	declare @insurer nvarchar(max)
	declare @product nvarchar(max)
	declare @accepted datetime
	--check the mathing data, insert new cases and update the existing ones
	WHILE EXISTS(SELECT TOP 1 * FROM #TEMP) 
		BEGIN 
			SELECT TOP 1 @policy = policy_ref, @version = version_ref, @handler = handler_key, @office = broker_office_key, @broker = broker_key, @client = client_key, @c_name = client_name, @product = product_target_name, @insurer = insurer_name, @accepted = accepted_date FROM #TEMP 
			
			INSERT INTO dbo.vt_live_breach_log (rule_code, context_ref, handler_key, office_key, team_key, broker_key, log_date) VALUES ('BOR', @policy, @handler, @office, @office, @broker, getdate())
			IF (select count(*) from dbo.vt_live_bor_archive where context_ref = @policy) = 0
				BEGIN
					insert into dbo.vt_live_bor_archive (context_ref, version_ref, client_key, client_name, insurer_name,product_target_name, accepted_date) 
												values (@policy, @version, @client, @c_name, @insurer, @product, @accepted)
				END
			ELSE
				BEGIN
					update dbo.vt_live_breach_log set handler_key = @handler, office_key = @office where rule_code='BOR' and context_ref=@policy
					update dbo.vt_live_bor_archive set version_ref = @version where context_ref=@policy
				END
			DELETE TOP(1) from #TEMP
		END
	DROP TABLE #TEMP
END

GO

