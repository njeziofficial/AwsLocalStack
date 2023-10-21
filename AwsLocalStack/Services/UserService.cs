using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using AwsLocalStack.Contracts;
using AwsLocalStack.Models;

namespace AwsLocalStack.Services
{
    public class UserService : IUserService
    {
        IDynamoDBContext _context;
        public UserService(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user)
        {
            user.Id = Guid.NewGuid().ToString();
            await _context.SaveAsync(user);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = GetUserById(id);
            if (user == null)
                return false;

            await _context.DeleteAsync(user);

            return true;
        }

        public User GetUserById(string id)
        {
            return   _context.ScanAsync<User>(default).GetRemainingAsync().Result.FirstOrDefault(x=>x.Id==id)!;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _context.ScanAsync<User>(default).GetRemainingAsync();
        }

        public async Task<bool> UpdateAsyc(User user)
        {
            var userInDB = GetUserById(user.Id);
            if (userInDB == null)
                return false;

            await _context.SaveAsync(user);
            return true;
        }
    }
}
