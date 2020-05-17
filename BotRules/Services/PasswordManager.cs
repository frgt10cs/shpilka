using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BotRules.Services
{
    public static class PasswordManager
    {        
        private static readonly Random rnd = new Random();
        private static readonly string symbols = "qwertyuioopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
        public static string MainSalt;

        /// <summary>
        /// Генерация соли для пароля
        /// </summary>
        /// <returns></returns>
        public static string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Случайна генерация строки. Макс длина - 20, мин -10
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateToken(int length = 10)
        {            
            // валидация
            length = length < 10 ? 10 : length;
            length = length > 20 ? 20 : length;

            string code = "";
            for (int i = 0; i < length; i++)
                code += symbols[new Random().Next(0, symbols.Length)];
            return code;
        }

        /// <summary>
        /// Генерация хэша из пароля и соли
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GenerateHash(string password, string salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt + MainSalt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256/8
                ));
        }
    }
}
