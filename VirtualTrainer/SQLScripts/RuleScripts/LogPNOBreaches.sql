
	declare @orgKey nvarchar(max), @orgName nvarchar(max)
	
	SET @orgKey = (SELECT top 1 o.organisation_key 
                  FROM <%ServerandDBName%>.[dbo].[organisation] o
                 WHERE o.organisation_key != -1)

	SET @orgName = (SELECT top 1 o.organisation_name 
                  FROM <%ServerandDBName%>.[dbo].[organisation] o
                 WHERE o.organisation_key != -1)

	CREATE TABLE #TEMP (	ContextRef nvarchar(10),
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
							RuleBreachFieldTwo money,
							RuleBreachFieldThree nvarchar(max), 
							RuleBreachFieldFour datetime, 
							RuleBreachFieldFive nvarchar(max),
							RuleBreachFieldSix nvarchar(100),
							RuleBreachFieldSeven datetime,
							RuleBreachFieldEight nvarchar(max)
						)

      insert into #temp select distinct p.policy_ref,
								h.user_ref,
								'Client ref: ' + CAST(p.client_key as nvarchar(max)) + ', Client Name: ' + cl.name + ', Policy Ref: ' + CAST(p.policy_ref as nvarchar(max)) + ', Oustanding Premium: ' + COALESCE(CAST(tr.total_insurer_outstanding as nvarchar(max)), '0.00') + ', Product: ' + t.product_target_name,
								'Client: ' + CAST(p.client_key as nvarchar(max)) + ', ' + cl.name + ', Policy: ' + CAST(p.policy_ref as nvarchar(max)),
								@orgKey,
								@orgName,
								p.policy_exec_office_key,
								o.task_exec_office_name,
								p.policy_income_team_key,
								team.team_name,
								p.client_key,
								COALESCE(tr.total_insurer_outstanding, 0.00),
								t.product_target_name,
								p.renewal_date_key,
								s.description,
								p.insurer_policy_no,
								p.accepted_date_key,
								cl.name
								from <%ServerandDBName%>.[dbo].policy p
								inner join <%ServerandDBName%>.[dbo].[policy_status] s on (p.policy_status_key = s.policy_status_key)
								inner join <%ServerandDBName%>.[dbo].[product_target] t on (p.product_target_key = t.product_target_key)
								inner join <%ServerandDBName%>.[dbo].[transaction] tr on (p.policy_key = tr.policy_key)
								inner join <%ServerandDBName%>.[dbo].[client] cl on (p.client_key = cl.client_key)
								inner join <%ServerandDBName%>.[dbo].[policy_type] pt on (t.policy_type_key = pt.policy_type_key)
								inner join <%ServerandDBName%>.[dbo].[category] c on (c.category_key = pt.category_key)
								inner join <%ServerandDBName%>.[dbo].[task_exec_office] as o on (p.[policy_exec_office_key] = o.[task_exec_office_key])
								inner join <%ServerandDBName%>.[dbo].[task_team] as team on (p.[policy_income_team_key] = team.task_team_key)
								inner join <%ServerandDBName%>.[dbo].[Account_handler] h on (p.policy_account_handler_key = h.account_handler_key)
								where (p.insurer_policy_no is NULL
                                    or p.insurer_policy_no like 'TBA%'
									or p.insurer_policy_no = 'TBA'
                                    or p.insurer_policy_no like 'TBC%'
                                    or p.insurer_policy_no like 'Not yet%'
                                    or p.insurer_policy_no like '%Unknown%'
                                    or p.insurer_policy_no like '%unknown%'
                                    or p.insurer_policy_no like '%N/A%'
                                    or p.insurer_policy_no = 'New'
                                    or p.insurer_policy_no like '%MYA%'
                                    or p.insurer_policy_no like '%T. B. A.%'
                                    or p.insurer_policy_no = 't b a'
                                    or p.insurer_policy_no = 'n y i'
                                    or p.insurer_policy_no = 'to be confirmed'
                                    or p.insurer_policy_no like '_'
                                    or p.insurer_policy_no = '' 
									or p.insurer_policy_no = 'To Be Advised'
                                    or p.insurer_policy_no = 'To be advised')
								and (s.description = 'Accepted' or s.description = 'Live')
								and p.cancellation_reason_key = -1
								and ((p.accepted_date_key > DATEADD(dd,-60,GETDATE()) and p.accepted_date_key <= DATEADD(dd,-30,GETDATE()))
                                   or (p.accepted_date_key <= DATEADD(dd,-60,GETDATE())))
								and t.product_target_name not like '%Add%'            -- No Add-Ons
								and c.category_name != 'Add On'
								and p.business_event_key <> 9 --no fees
								and tr.total_insurer_outstanding <> 0
								and p.policy_ref not in (
									select * FROM dbo.SplitString(COALESCE(NULLIF(@excludedContextRefs,''), '00000'), ',')
								)
								and p.product_target_key not in (
									select * FROM dbo.SplitString(COALESCE(NULLIF(@excludedProductKeys,''), '00000'), ',')
								)
								and p.policy_agent_key not in (
									select * FROM dbo.SplitString(COALESCE(NULLIF(@excludedAgentKeys,''), '00000'), ',')
								)
	select * from #Temp