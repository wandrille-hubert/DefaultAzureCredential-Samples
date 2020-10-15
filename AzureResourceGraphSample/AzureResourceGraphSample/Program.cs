using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Microsoft.Rest;

namespace AzureResourceGraphSample
{
	class Program
	{
		// The subscription id against which the query shall be ran
		const string SubscriptionId = "SUbSCRIPTION-ID";

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


			// Create token request context with scope set to .default
			TokenRequestContext tokenRequestContext = new TokenRequestContext(new[] { "https://management.core.windows.net//.default" });


			// Get token which will sequentially call the included credentials in the order
			// EnvironmentCredential, ManagedIdentityCredential, SharedTokenCacheCredential,
			// and InteractiveBrowserCredential returning the first successfully obtained AccessToken
			// for more information: https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential.gettokenasync?view=azure-dotnet
			AccessToken tokenRequestResult = await credential.GetTokenAsync(tokenRequestContext);


			// Initialize ServiceClientCredential utilizing acquired token
			ServiceClientCredentials serviceClientCreds = new TokenCredentials(tokenRequestResult.Token);


			// Initialize ResourceGraphClient with the credentials
			ResourceGraphClient argClient = new ResourceGraphClient(serviceClientCreds);


			// Create query that will get max 5 resources found in the subscription 'SubscriptionId'
			QueryRequest request = new QueryRequest();
			request.Subscriptions = new List<string>() { SubscriptionId };
			request.Query = "Resources | project name, type | limit 5";


			// Send query to the ResourceGraphClient and get response
			QueryResponse response = argClient.Resources(request);


			// Print out results to the console
			// When deployed as a trigger webjob, by default
			// these will be written out to the webjob run log
			Console.WriteLine("Records: " + response.Count);
			Console.WriteLine("Data:\n" + response.Data);
		}

	}
}
