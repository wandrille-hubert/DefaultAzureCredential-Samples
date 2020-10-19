#Requires -PSEdition Desktop -Modules AzureAD
Connect-AzureAD -TenantId $(Read-Host -Prompt "TenantId")
# GraphAggregatorService = 00000003-0000-0000-c000-000000000000 the MS Graph's multi-tenant app
$graph = Get-AzureADServicePrincipal -Filter "AppId eq '00000003-0000-0000-c000-000000000000'"
$groupReadPermission = $graph.AppRoles `
    | Where-Object Value -Like "Application.Read.All" `
    | Select-Object -First 1

$msi = Get-AzureADServicePrincipal -ObjectId $(Read-Host -Prompt "Managed Identity ObjectId")
# https://docs.microsoft.com/en-us/azure/active-directory/users-groups-roles/directory-assign-admin-roles#privileged-role-administrator-permissions
Write-Warning -Message "Note: Privileged Role Administrator required for any permission that requires admin consent."
New-AzureADServiceAppRoleAssignment `
    -Id $groupReadPermission.Id `
    -ObjectId $msi.ObjectId `
    -PrincipalId $msi.ObjectId `
    -ResourceId $graph.ObjectId
