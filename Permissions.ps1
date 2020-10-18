Connect-AzureAD -TenantId dd218d36-09e8-4126-85e2-33eff0793b72
$graph = Get-AzureADServicePrincipal -Filter "AppId eq '00000003-0000-0000-c000-000000000000'"
$groupReadPermission = $graph.AppRoles `
    | where Value -Like "Application.Read.All" `
    | Select-Object -First 1

$msi = Get-AzureADServicePrincipal -ObjectId 61f91070-8bfc-4e1e-883c-e8b27df776ff

New-AzureADServiceAppRoleAssignment `
    -Id $groupReadPermission.Id `
    -ObjectId $msi.ObjectId `
    -PrincipalId $msi.ObjectId `
    -ResourceId $graph.ObjectId