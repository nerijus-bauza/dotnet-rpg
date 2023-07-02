using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Data
{
  public class AuthRepository : IAuthRepository
  {
    private readonly DataContext context;
    public AuthRepository(DataContext context)
    {
      this.context = context;
    }
    public Task<ServiceResponse<string>> Login(string username, string password)
    {
      throw new NotImplementedException();
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
      var serviceResponse = new ServiceResponse<int>();
      if (await UserExists(user.Username))
      {
        serviceResponse.Success = false;
        serviceResponse.Message = $"User '{user.Username}' already exists.";
        return serviceResponse;
      }
      
      CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;
      
      context.Users.Add(user);
      await context.SaveChangesAsync();
      
      serviceResponse.Data = user.Id;
      serviceResponse.Message = $"User '{user.Username}' successfully created";

      return serviceResponse;
    }

    public async Task<bool> UserExists(string username)
    {
      if (await context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
      {
        return true;
      }

      return false;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using(var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
  }
}