using Ibuy.Models;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace Ibuy.Service
{

    //Hashs User password, encrpytion
    public class PasswordServices
    {
        private  readonly PasswordHasher<string> _hasher = new();

        public  string HashPassword(string password)
        {
            return _hasher.HashPassword("user", password);
        }

        public  bool VerifyPassword(string hashedPassword, string password)
        {
            var res = _hasher.VerifyHashedPassword("user", hashedPassword, password);
            return res == PasswordVerificationResult.Success;
        }
    }


}