using AwsLocalStack.Models;

namespace AwsLocalStack.Contracts
{
    public interface IUserService
    {
        Task CreateAsync(User user);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsyc(User user);
        Task<List<User>> GetUsers();
        User GetUserById(string id);
    }
}
