using Amazon.DynamoDBv2.DataModel;

namespace AwsLocalStack.Models;

[DynamoDBTable("users")]
public class User
{
    [DynamoDBHashKey("id")]
    public string? Id { get; set; }
    [DynamoDBProperty("first_name")]
    public string? FirstName { get; set; }
    [DynamoDBProperty("last_name")]
    public string? LastName { get; set; }
    [DynamoDBProperty("phone_number")]
    public string? PhoneNumber { get; set; }
}
