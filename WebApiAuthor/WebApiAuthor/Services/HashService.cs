using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WebApiAuthor.DTOs;

namespace WebApiAuthor.Services
{
    public class HashService
    {
        public HashResult Hash(string planeText)
        {
            var sal = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(sal);
            }

            return Hash(planeText, sal);
        }

        public HashResult Hash(string planeText, byte[] sal)
        {
            var derivedKey = KeyDerivation.Pbkdf2(password: planeText,
                salt: sal, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 32);

            var hash = Convert.ToBase64String(derivedKey);

            return new HashResult()
            {
                Hash = hash,
                Sal = sal
            };
        }
    } 
}

