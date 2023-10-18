using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System.Reflection;

namespace AwsLocalStack.Helpers;

public class DynamoDbTableCreator
{
    IAmazonDynamoDB _dynamoDbClient;

    public DynamoDbTableCreator(IAmazonDynamoDB dynamoDbClient)
    {
        _dynamoDbClient = dynamoDbClient;
    }

    public async Task CreateTableAsync<T>()
    {
        Type type = typeof(T);
        DynamoDBTableAttribute tableAttribute = type.GetCustomAttribute<DynamoDBTableAttribute>()!;
        if (tableAttribute == null)
        {
            throw new InvalidOperationException("The provided class does not have a dynamoDBTable attribute");
        }

        string tableName = tableAttribute.TableName;
        if (await DoesTableExistAsync(tableName))
        {
            Console.WriteLine($"Table '{tableName}' already exists. Skipping table creation.");
            return;
        }

        //Create Table if it does not exist.
        var attributeDefinitions = new List<AttributeDefinition>();
        var keySchema = new List<KeySchemaElement>();
        foreach (var property in type.GetProperties())
        {
            DynamoDBHashKeyAttribute hashKeyAttribute = property.GetCustomAttribute<DynamoDBHashKeyAttribute>()!;
            DynamoDBRangeKeyAttribute rangeKeyAttribute = property.GetCustomAttribute<DynamoDBRangeKeyAttribute>()!;
            if (hashKeyAttribute != null || rangeKeyAttribute != null)
            {
                string attributeName = hashKeyAttribute.AttributeName;
                string attributeType = property.PropertyType == typeof(string) ? "S" : "N";
                attributeDefinitions.Add(new AttributeDefinition
                {
                    AttributeName = attributeName,
                    AttributeType = attributeType
                });

                keySchema.Add(new KeySchemaElement
                {
                    AttributeName = attributeName,
                    KeyType = hashKeyAttribute != null ? "HASH" : "RANGE"
                });
            }
        }

        var request = new CreateTableRequest
        {
            TableName = tableName,
            AttributeDefinitions = attributeDefinitions,
            KeySchema = keySchema,
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 5,
                WriteCapacityUnits = 5
            }
        };

        await _dynamoDbClient.CreateTableAsync(request);
    }

    private async Task<bool> DoesTableExistAsync(string tableName)
    {
         var tables = await _dynamoDbClient.ListTablesAsync();
        return tables.TableNames.Contains(tableName);
    }
}
