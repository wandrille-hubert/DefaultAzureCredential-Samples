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
	class Program
	{
		// The subscription id against which the query shall be ran
		const string SubscriptionId = "SUBSCRIPTION-ID";

		// The current example is set up to utilize a system assigned managed identity.
		// If you would like to use a user assigned managed identity, then go ahead and
		// uncomment this line, and initialize DefaultAzureCredential utilizing
		// this value which is the Id of the user assigned managed identity
		//const string UserAssignedManagedIdentityId = "USER-ASSIGNED-MANAGED-IDENTITY-ID";

		static async Task Main(string[] args)
		{
			ResourceFinder rf = new ResourceFinder(SubscriptionId);
			await ResourceFinder.FindResources();
		}
	}
}
