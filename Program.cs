using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    // These should be moved to configuration in a production environment
    private static readonly string ConnectionString = "";
    private static readonly string DatabaseId = "flexible-report";
    private static readonly string ContainerId = "flexible-report";

    static async Task Main(string[] args)
    {
        try
        {
            using CosmosClient cosmosClient = new(ConnectionString, new CosmosClientOptions { AllowBulkExecution = true });
            Container container = cosmosClient.GetContainer(DatabaseId, ContainerId);

            // Get all items from the container
            var query = new QueryDefinition("SELECT * FROM c");
            var iterator = container.GetItemQueryIterator<dynamic>(query);

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var item in response)
                {
                    // Convert the item to JObject for easy property manipulation
                    JObject jsonObject = JObject.Parse(item.ToString());

                    // Convert all properties recursively to camelCase
                    var camelCaseObject = ConvertToCamelCase(jsonObject);

                    // Get the id from the original object
                    string id = jsonObject["id"]?.ToString() ?? throw new Exception("Item must have an id");
                    
                    // Replace the item in Cosmos DB without partition key
                    await container.UpsertItemAsync(
                        camelCaseObject,
                        requestOptions: new ItemRequestOptions { EnableContentResponseOnWrite = false }
                    );

                    Console.WriteLine($"Processed item with id: {id}");
                }
            }

            Console.WriteLine("All items have been processed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static JObject ConvertToCamelCase(JObject original)
    {
        var result = new JObject();

        foreach (var property in original.Properties())
        {
            string camelCaseName = ToCamelCase(property.Name);
            JToken newValue = property.Value;

            // Recursively process nested objects and arrays
            if (property.Value is JObject nestedObject)
            {
                newValue = ConvertToCamelCase(nestedObject);
            }
            else if (property.Value is JArray array)
            {
                newValue = ConvertArrayToCamelCase(array);
            }

            result.Add(camelCaseName, newValue);
        }

        return result;
    }

    private static JArray ConvertArrayToCamelCase(JArray original)
    {
        var result = new JArray();

        foreach (var item in original)
        {
            if (item is JObject obj)
            {
                result.Add(ConvertToCamelCase(obj));
            }
            else if (item is JArray arr)
            {
                result.Add(ConvertArrayToCamelCase(arr));
            }
            else
            {
                result.Add(item);
            }
        }

        return result;
    }

    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        if (str.Length == 1)
            return str.ToLower();

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
