using Amazon.DynamoDBv2.DataModel;

namespace AwsLocalStack.Models
{
    public class UserInfo
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
