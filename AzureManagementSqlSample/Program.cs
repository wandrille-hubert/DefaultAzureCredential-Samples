using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Sql.Fluent;
using Microsoft.Rest;

namespace AzureManagementSqlSample
{
	class Program
	{
		static async Task Main(string[] args)
		{
			string tenantId = "00000000-0000-0000-0000-000000000000";
			string subscriptionId = "00000000-0000-0000-0000-000000000000";

			DefaultAzureCredential credential = new DefaultAzureCredential();

			// If utilizing a user assigned managed identity, then uncomment the second line and comment out the line above
			// DefaultAzureCredential credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = UserAssignedManagedIdentityId });

			// Create token request context with scope set to .default for the ARM and MS Graph endpoints
			// Get token which will sequentially call the included credentials in the order
			// EnvironmentCredential, ManagedIdentityCredential, SharedTokenCacheCredential,
			// and InteractiveBrowserCredential returning the first successfully obtained AccessToken
			// for more information: https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential.gettokenasync?view=azure-dotnet
			TokenRequestContext tokenRequestContextARM = new TokenRequestContext(new[] { "https://management.core.windows.net//.default" });
			AccessToken tokenRequestResultARM = await credential.GetTokenAsync(tokenRequestContextARM);
			TokenRequestContext tokenRequestContextGraph = new TokenRequestContext(new[] { "https://graph.microsoft.com//.default" });
			AccessToken tokenRequestResultGraph = await credential.GetTokenAsync(tokenRequestContextGraph);

			// Credentials used for authenticating a fluent management client to Azure.
			AzureCredentials credentials = new AzureCredentials(
								new TokenCredentials(tokenRequestResultARM.Token),
								new TokenCredentials(tokenRequestResultGraph.Token),
								tenantId,
								AzureEnvironment.AzureGlobalCloud);

			// Top level abstraction of Azure. https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.management.fluent.iazure?view=azure-dotnet
			// .WithSubscription is optional if you wish to return resource beyond the scope of a single subscription.
			IAzure azure = Microsoft.Azure.Management.Fluent.Azure
							.Configure()
							.WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
							.Authenticate(credentials)
							.WithSubscription(subscriptionId);

			// Iterate through Microsoft.Sql top level resources (servers) and a list of databases (sub resources)
			// for data collection define IList<T> outside of these nested loops and add resources and sub resources
			// of interest to collections.
			IPagedCollection<ISqlServer> servers = await azure.SqlServers.ListAsync();
			do {
				foreach (ISqlServer server in servers)
				{
					Console.WriteLine($"Server: {server.Name}  Azure AD Admin: {server.GetActiveDirectoryAdministrator().SignInName}");
					Console.WriteLine($"\tResource Group: {server.ResourceGroupName}");

					IReadOnlyList<ISqlDatabase> databases = await server.Databases.ListAsync();
					foreach (ISqlDatabase database in databases)
					{
						Console.WriteLine($"\t\tDatabase name: {database.Name}  created: {database.CreationDate}");
					}
				}
				servers = await servers.GetNextPageAsync();
			} while (servers != null);
		}
	}
}
