# AzureGraphSample

This sample demonstrates the usage of the new [Azure.Identity](https://www.nuget.org/packages/Azure.Identity) package and utilizing it to authenticate with the Microsoft.Graph package.  In this case, this console app demonstrates the usage of [DefaultAzureCredential](https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) to be able to then query the Microsoft Graph api and return a list of applications.
The sample can be run both locally as well as deployed to Azure.

## Local Run
In order for the application to be run locally, you will need to have set up authentication either through Visual Studio, Visual Studio Code, or the Azure CLI.  More information can be found at: [Authenticate The Client](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme#authenticate-the-client)


## Deploy To Azure
Publish the sample as an Azure Webjob (trigger) on an App Service.  By default, an app service will have created a system assigned managed identity. In order for this sample to be ran correctly,
the managed identity will need Graph Api permissions.  Unfortunately, it is not currently possible to set this from the Azure portal.
However, the following powershell script can be utilized to grant the needed permissions.
This script does require the AzureAD module to be installed.  If not installed, just run the following to install it.
```
Install-Module AzureAD
```

Make sure to replace the WEB APP MSI Identity with your managed identity value.

```
Connect-AzureAD
$graph = Get-AzureADServicePrincipal -Filter "AppId eq '00000003-0000-0000-c000-000000000000'"
$groupReadPermission = $graph.AppRoles `
    | where Value -Like "Application.Read.All" `
    | Select-Object -First 1

$msi = Get-AzureADServicePrincipal -ObjectId <WEB APP MSI Identity>

New-AzureADServiceAppRoleAssignment `
    -Id $groupReadPermission.Id `
    -ObjectId $msi.ObjectId `
    -PrincipalId $msi.ObjectId `
    -ResourceId $graph.ObjectId
```

Once setup correctly, go ahead and trigger the Webjob that you published.  To validate the success and results, view the logs of the webjob to view the output as Console.Write will write to the logs for a trigger webjob.

## User Assigned Managed Identity
A user assigned managed identity can also be utilized instead of the system assigned managed identity.  In order to use this, within the program go ahead and uncomment the property UserAssignedManagedIdentityId and set it with the Id of the user assigned managed identity.  Then uncomment the initialization of the DefaultAzureCredential utilizing the UserAssignedManagedIdentityId.  Then make sure that the user assigned managed identity has been assigned on the App Service and the proper permissions have been given to the managed identity, by running the script above and make sure to provide the ObjectId of the managed identity.
