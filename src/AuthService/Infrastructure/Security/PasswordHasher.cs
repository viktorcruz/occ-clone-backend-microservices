using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace AuthService.Infrastructure.Security
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            var salt = new byte[SaltSize];
            new SecureRandom().NextBytes(salt);

            var generator = new Pkcs5S2ParametersGenerator();
            generator.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()), salt, Iterations);

            var key = (KeyParameter)generator.GenerateDerivedMacParameters(KeySize * 8); 
            var hash = key.GetKey();

            var hashBytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var hashBytes = Convert.FromBase64String(storedHash);

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var generator = new Pkcs5S2ParametersGenerator();
            generator.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()), salt, Iterations);

            var key = (KeyParameter)generator.GenerateDerivedMacParameters(KeySize * 8);
            var derivedHash = key.GetKey();

            for (int i = 0; i < KeySize; i++)
            {
                if (hashBytes[i + SaltSize] != derivedHash[i])
                {
                    return false; 
                }
            }

            return true; 
        }
    }
}
