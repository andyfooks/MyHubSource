select	ou.[organisation_user_key] as UserKey,
		ou.[user_name] as UserName,
		ou.[email] as UserEmail,
		ou.[user_role] as UserRole,
		ou.[user_status_key] as UserStatusKey,
		us.[description] as UserStatusDescription,
		ot.[organisation_team_key] as TeamKey,
		ot.[team_name] as TeamName,
		ot.[team_status_key] as TeamStatusKey,
		ts.[description] as TeamStatusDescription,
		oo.[organisation_office_key] as OfficeKey,
		oo.[organisation_office_name] as OfficeName,
		o.[organisation_key] as OrganisationKey,
		o.[organisation_name] as OrganisationName
		
from <%ServerandDBName%>.[dbo].[organisation_user] as ou with(nolock)
inner join <%ServerandDBName%>.[dbo].[user_status] as us with(nolock) on us.[user_status_key] = ou.[user_status_key]
inner join <%ServerandDBName%>.[dbo].[organisation_team] as ot with(nolock) on ot.[organisation_team_key] = ou.[organisation_team_key]
inner join <%ServerandDBName%>.[dbo].[team_status] as ts with(nolock) on ts.[team_status_key] = ot.[team_status_key]
inner join <%ServerandDBName%>.[dbo].[organisation_office] as oo with(nolock) on oo.[organisation_office_key] = ot.[organisation_office_key]
inner join <%ServerandDBName%>.[dbo].[organisation] as o with(nolock) on o.[organisation_key] = oo.[organisation_key]

where ou.[organisation_user_key] <> -1
and ot.[organisation_team_key] <> -1
and oo.[organisation_office_key] <> -1
and oo.[organisation_key] <> -1
order by ou.email