using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;
using Azure.Core;
using System.Collections.Generic;

namespace AzureGraphSample
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


			// Create token request context with scope set to .default
			TokenRequestContext tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });


			// Get token which will sequentially call the included credentials in the order
			// EnvironmentCredential, ManagedIdentityCredential, SharedTokenCacheCredential,
			// and InteractiveBrowserCredential returning the first successfully obtained AccessToken
			AccessToken token = credential.GetToken(tokenRequestContext);
			string accessToken = token.Token;


			// Initialize the auth provider with the access token acquired
			DelegateAuthenticationProvider authProvider = new DelegateAuthenticationProvider((requestMessage) =>
			{
				requestMessage
				.Headers
				.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
				return Task.CompletedTask;
			});


			// Initialize graph client with the auth provider
			GraphServiceClient graphServiceClient = new GraphServiceClient(authProvider);


			// Get CollectionPage of applications
			var applicationList = new List<Application>();
			IGraphServiceApplicationsCollectionPage applications = await graphServiceClient.Applications.Request().GetAsync();
			applicationList.AddRange(applications.CurrentPage);


			// Cycle through all pages to get all applications
			while (applications.NextPageRequest != null)
			{
				applications = await applications.NextPageRequest.GetAsync();
				applicationList.AddRange(applications.CurrentPage);
			}


			// Print out applications to the console
			// When deployed as a trigger webjob, by default
			// these will be written out to the webjob run log
			foreach (var app in applicationList)
			{
				Console.WriteLine("Application: " + app.DisplayName);
			}

		}
	}
}
