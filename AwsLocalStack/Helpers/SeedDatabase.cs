using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AwsLocalStack.Models;

namespace AwsLocalStack.Helpers;

public static class SeedDatabase
{
    public static async Task<IServiceCollection> Initialize(this IServiceCollection services, IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dynamoDbClient = scope.ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
            var dynamoDbContext = scope.ServiceProvider.GetRequiredService<IDynamoDBContext>();
            var dynamoDbTableCreator = new DynamoDbTableCreator(dynamoDbClient);
            await dynamoDbTableCreator.CreateTableAsync<User>();
            await DatabaseSeeder(dynamoDbContext);
            return services;
        }


    }

    private static async Task DatabaseSeeder(IDynamoDBContext context)
    {
        bool isUserPresent = (await context.ScanAsync<User>(default).GetRemainingAsync()).Count() > 0;
        if (!isUserPresent)
        {
            foreach (var user in GetUsers())
            {
                await context.SaveAsync(user);
            }
        }

        return;
    }

    private static List<User> GetUsers()
    {
        return new List<User>
        {
            new User{ Id = "1", FirstName = "Fregene", LastName = "Tom", PhoneNumber = "9585785758"},
            new User{ Id = "2", FirstName = "Mark", LastName = "Ant", PhoneNumber = "89588676837"},
            new User{ Id = "3", FirstName = "Frank", LastName = "Idohu", PhoneNumber = "123454345"}
        };
    }
}
