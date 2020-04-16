
	declare @orgKey nvarchar(max), @orgName nvarchar(max)
	
	SET @orgKey = (SELECT top 1 o.organisation_key
                  FROM <%ServerandDBName%>.[dbo].[organisation] o
                 WHERE o.organisation_key != -1)

	SET @orgName = (SELECT top 1 o.organisation_name
                  FROM <%ServerandDBName%>.[dbo].[organisation] o
                 WHERE o.organisation_key != -1)

   
	create table #TEMP( ContextRef nvarchar(10),
						AccountHandlerUserKey nvarchar(max),
						BreachDisplayText nvarchar(max),
						BreachDisplayAlternateText nvarchar(max),
						ActurisOrganisationKey int, 
						ActurisOrganisationName varchar(max),
						OfficeKey int,
						OfficeName varchar(max),
						TeamKey int,
						TeamName varchar(max),
						RuleBreachFieldOne int,				
						RuleBreachFieldTwo nvarchar(100),	
						RuleBreachFieldFour nvarchar(60),	
						RuleBreachFieldThree nvarchar(60),	
						RuleBreachFieldFive nvarchar(60),	
						RuleBreachFieldSix nvarchar(60),	
						RuleBreachFieldSeven varchar(20),	
					)

	insert into #TEMP   select distinct 
						c.client_key, 
						h.user_ref,
						'Client ref: ' + CAST(c.client_key as nvarchar(max)) + ', Client Name: ' + c.name + ',  Salutation: ' + COALESCE(CAST(pc.salutation as nvarchar(max)), '') + ', Home Phone: ' + COALESCE(CAST(pc.home_phone as nvarchar(max)), '') + ', Mobile: ' + COALESCE(CAST(pc.mobile as nvarchar(max)), '') + ', Email: ' + COALESCE(CAST(pc.email as nvarchar(max)), ''),
						'Client: ' + CAST(p.client_key as nvarchar(max)) + ', ' + c.name + ', Policy: ' + CAST(p.policy_ref as nvarchar(max)),
						@orgKey,
						@orgName,
						p.policy_exec_office_key,
						o.task_exec_office_name,
						p.policy_income_team_key,
						team.team_name,
						pc.[contact_people_key], 
						pc.salutation, 
						pc.home_phone, 
						pc.work_phone, 
						pc.mobile, 
						pc.email, 
						pc.dob_date
						from <%ServerandDBName%>.[dbo].[client] c
						inner join <%ServerandDBName%>.[dbo].[contact_people] pc on pc.client_key = c.client_key
						inner join <%ServerandDBName%>.[dbo].[policy] p on (p.client_key = c.client_key)
						inner join <%ServerandDBName%>.[dbo].[task_exec_office] as o on (p.[policy_exec_office_key] = o.[task_exec_office_key])
						inner join <%ServerandDBName%>.[dbo].[task_team] as team on (p.[policy_income_team_key] = team.task_team_key)
						inner join <%ServerandDBName%>.[dbo].[Account_handler] h on (p.policy_account_handler_key = h.account_handler_key)
						where
						pc.primary_indicator_key = 1
						-- fail criteria
						and ((
								pc.salutation is null or (pc.home_phone is null and pc.work_phone is null and pc.mobile is null) 
							) or (
								pc.email not like '%_@_%.__%'
							)
						)
						and ( p.policy_status_key = 7 or p.policy_status_key = 8 )
						and p.cancellation_reason_key = -1
						and p.business_event_key <> 9 --no fees
						and ((
								p.accepted_date_key >= dateadd(dd, -14, getdate())
								and p.accepted_date_key <= dateadd(dd, -7, getdate())
							)
						)
						and c.client_key not in (
							select * FROM dbo.SplitString(COALESCE(NULLIF(@excludedContextRefs,''), '00000'), ',')
						)
						and p.product_target_key not in (
							select * FROM dbo.SplitString(COALESCE(NULLIF(@excludedProductKeys,''), '00000'), ',')
						)
						and p.policy_agent_key not in (
							select * FROM dbo.SplitString(COALESCE(NULLIF(@excludedAgentKeys,''), '00000'), ',')
						)

	select * from #TEMP