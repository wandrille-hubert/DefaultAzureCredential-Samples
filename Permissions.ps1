﻿Connect-AzureAD -TenantId dd218d36-09e8-4126-85e2-33eff0793b72
$graph = Get-AzureADServicePrincipal -Filter "AppId eq '00000003-0000-0000-c000-000000000000'"
$groupReadPermission = $graph.AppRoles `
    | where Value -Like "Application.Read.All" `
    | Select-Object -First 1

$msi = Get-AzureADServicePrincipal -ObjectId MANAGEDIDENTITYOBJECTID

New-AzureADServiceAppRoleAssignment `
    -Id $groupReadPermission.Id `
    -ObjectId $msi.ObjectId `
    -PrincipalId $msi.ObjectId `
    -ResourceId $graph.ObjectId