using Appwrite;
using Appwrite.Services;
using Appwrite.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using avaloniaappwritetrae20260119.Models;
using System.Linq;
using System.Text.Json;
using System;

namespace avaloniaappwritetrae20260119.Services
{
    public class AppwriteService
    {
        private readonly Client _client;
        private readonly Databases _databases;

        private const string Endpoint = "https://fra.cloud.appwrite.io/v1";
        private const string ProjectId = "680c76af0037a7d23e44";
        private const string DatabaseId = "680c778b000f055f6409";
        private const string CollectionId = "687250d70020221fb26c";

        public AppwriteService()
        {
            _client = new Client()
                .SetEndpoint(Endpoint)
                .SetProject(ProjectId);

            _databases = new Databases(_client);
        }

        public async Task<List<Subscription>> GetSubscriptionsAsync()
        {
            try
            {
                // Query.OrderAsc("nextdate") to sort by date near to far
                var documentList = await _databases.ListDocuments(
                    databaseId: DatabaseId,
                    collectionId: CollectionId,
                    queries: new List<string> { Query.OrderAsc("nextdate") }
                );

                var subscriptions = new List<Subscription>();

                foreach (var doc in documentList.Documents)
                {
                    try 
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var json = JsonSerializer.Serialize(doc.Data);
                        var subscription = JsonSerializer.Deserialize<Subscription>(json, options);

                        if (subscription != null)
                        {
                            subscription.Id = doc.Id;
                            subscription.CreatedAt = !string.IsNullOrEmpty(doc.CreatedAt) ? DateTime.Parse(doc.CreatedAt) : DateTime.MinValue;
                            subscription.UpdatedAt = !string.IsNullOrEmpty(doc.UpdatedAt) ? DateTime.Parse(doc.UpdatedAt) : DateTime.MinValue;
                            subscriptions.Add(subscription);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error mapping subscription: {ex.Message}");
                    }
                }

                return subscriptions;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching subscriptions: {ex.Message}");
                return new List<Subscription>();
            }
        }
    }
}
