using System;
using Azure.Identity;
using Azure.ResourceManager.Resources;
using Microsoft.Azure.Management.Subscription;
using Azure.Core;
using Microsoft.Rest;
using System.Threading.Tasks;
using Azure.ResourceManager.Resources.Models;
using Azure;

namespace AzureIdentitySample
{
	class Program
	{
		// The current example is set up to utilize a system assigned managed identity.
		// If you would like to use a user assigned managed identity, then go ahead and
		// uncomment this line, and initialize DefaultAzureCredential utilizing
		// this value which is the Id of the user assigned managed identity
		//const string UserAssignedManagedIdentityId = "USER-ASSIGNED-MANAGED-IDENTITY-ID";

		static async Task Main(string[] args)
		{
			// Initialize a DefaultAzureCredential which is a part of the
			// Azure.Identity package which provides an authentication flow
			// for more information: https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet
			DefaultAzureCredential credential = new DefaultAzureCredential();


			// If utilizing a user assigned managed identity, then uncomment the
			// second line and comment out the first line
			//var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = UserAssignedManagedIdentityId });

			
			// Call to function to get all subscriptions as well as all resource groups in the subscription
			await GetSubscriptions(credential);
		}


		static async Task GetSubscriptions(DefaultAzureCredential credential)
		{
			// Create token request context with scope set to .default
			TokenRequestContext tokenRequestContext = new TokenRequestContext(new[] { "https://management.core.windows.net//.default" });


			// Get token which will sequentially call the included credentials in the order
			// EnvironmentCredential, ManagedIdentityCredential, SharedTokenCacheCredential,
			// and InteractiveBrowserCredential returning the first successfully obtained AccessToken
			// for more information: https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential.gettokenasync?view=azure-dotnet
			AccessToken tokenRequestResult = await credential.GetTokenAsync(tokenRequestContext);


			// Initialize ServiceClientCredential utilizing acquired token
			ServiceClientCredentials serviceClientCreds = new TokenCredentials(tokenRequestResult.Token);


			// Initialize SubscriptionClient with the service client credentials
			SubscriptionClient subClient = new SubscriptionClient(serviceClientCreds);


			// Get list of subscriptions in tenant
			var listSubscriptions = subClient.Subscriptions.List();


			// Print out results to the console
			foreach (var sub in listSubscriptions)
			{
				Console.WriteLine("Subscription: " + sub.DisplayName);

				// Get all resource groups in the subscription
				await GetResourceGroups(credential, sub.SubscriptionId);
				Console.WriteLine("\n");
			}
		}

		static async Task GetResourceGroups(DefaultAzureCredential credential, string subscriptionId)
		{
			// Create the resource client that will be used to fetch the resource groups
			ResourcesManagementClient resourceClient = new ResourcesManagementClient(subscriptionId, credential);


			// Fetch the resource groups and print them out to the screen
			Pageable<ResourceGroup> listResourceGroups = resourceClient.ResourceGroups.List();


			// Print out results to the console
			foreach (var Group in listResourceGroups)
			{
				Console.WriteLine("Resource group: " + Group.Name);
			}
		}
	}
}
