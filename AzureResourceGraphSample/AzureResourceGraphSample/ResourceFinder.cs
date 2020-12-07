using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;

namespace AzureResourceGraphSample
{
	public class ResourceFinder
	{
		public ResourceFinder()
		{

		}

		public ResourceFinder(string _SubscriptionId)
		{
			SubscriptionId = _SubscriptionId;
		}

		public static string SubscriptionId { get; set; }

		public static async Task FindResources()
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
			request.Query = "Resources | project id, name, type";
			request.Options = new QueryRequestOptions() { Top = 5, ResultFormat = ResultFormat.ObjectArray };
			// NOTE: top 5 is being passed in here to 'force' the implementation of pagination.  It can be excluded
			// and then will default to 1000 items returned.
			// ResultFormat can be set to Table or ObjectArray (and defaults to Table if not set).
			// More info can be found at https://docs.microsoft.com/en-us/azure/governance/resource-graph/concepts/work-with-data#formatting-results


			// Parameter to hold full list of returned resources
			var results = new List<ResourceItem>();


			// Send query to the ResourceGraphClient and get response
			QueryResponse response = argClient.Resources(request);


			// IMPORTANT: The query must project the id field in order for pagination to work.
			// If it's missing from the query, the response won't include the $skipToken.
			if (response.Count > 0)
			{
				// Add response results to list
				results.AddRange(((JArray)response.Data).ToObject<List<ResourceItem>>());

				// Continue till SkipToken is null
				while (!string.IsNullOrWhiteSpace(response.SkipToken))
				{
					// Update request with new skip token returned from response
					request.Options.SkipToken = response.SkipToken;

					// Send query with SkipToken to the ResourceGraphClient and get response
					response = argClient.Resources(request);

					// Add response results to list
					results.AddRange(((JArray)response.Data).ToObject<List<ResourceItem>>());
				}
			}


			// Print out results to the console
			// When deployed as a trigger webjob, by default
			// these will be written out to the webjob run log
			Console.WriteLine("List of resource names found:");
			foreach (var res in results)
			{
				Console.WriteLine(res.name);
			}
		}

	}
}
