# AzureResourceGraphSample

This sample demonstrates the usage of the new [Azure.Identity](https://www.nuget.org/packages/Azure.Identity) package and utilizing to authenticate with the older Microsoft.Azure.Management.* libraries.  In this case, this console app demonstrates the usage of [DefaultAzureCredential](https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) to then be able to utilize the [Microsoft.Azure.Management.ResourceGraph](https://www.nuget.org/packages/Microsoft.Azure.Management.ResourceGraph/) in order to return resources found within an Azure subscription.  
The sample can be run both locally as well as deployed to Azure.  To begin, fill in the SubscriptionId.

## Local Run
In order for the application to be run locally, you will need to have set up authentication either through Visual Studio, Visual Studio Code, or the Azure CLI.  More information can be found at: [Authenticate The Client](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme#authenticate-the-client)


## Deploy To Azure
Publish the sample as an Azure Webjob (trigger) on an App Service.  By default, an app service will have created a system assigned managed identity. In order for this sample to be ran correctly,
the managed identity will need to at least have Read permissions on one resource.  For the sake of this sample, you can easily assign the role of Reader to the managed identity from within the Azure portal.

Once setup correctly, go ahead and trigger the Webjob that you published.  To validate the success and results, view the logs of the webjob to view the output as Console.Write will write to the logs for a trigger webjob.

A user assigned managed identity can also be utilized instead of the system assigned managed identity.  In order to use this, within the program go ahead and uncomment the property UserAssignedManagedIdentityId and set it with the Id of the user assigned managed identity.  Then uncomment the initialization of the DefaultAzureCredential utilizing the UserAssignedManagedIdentityId.  Then make sure the managed identity has at least Read permissions on a resource.
