Connect-AzureAD -TenantId dd218d36-09e8-4126-85e2-33eff0793b72
$graph = Get-AzureADServicePrincipal -Filter "AppId eq '00000003-0000-0000-c000-000000000000'"
$groupReadPermission = $graph.AppRoles `
    | Where-Object Value -Like "Application.Read.All" `
    | Select-Object -First 1

$msi = Get-AzureADServicePrincipal -ObjectId 216f5e26-a270-4390-b8fb-7d70dbc9d6b7

New-AzureADServiceAppRoleAssignment `
    -Id $groupReadPermission.Id `
    -ObjectId $msi.ObjectId `
    -PrincipalId $msi.ObjectId `
    -ResourceId $graph.ObjectId
