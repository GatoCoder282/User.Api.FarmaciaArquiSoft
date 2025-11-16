using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Ports;

namespace User.Application.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService()
        {

            string keySource = "ESTA ES MI CLAVE SECRETA DE 32 BYTES PARA AES-256!";
            string ivSource = "MI IV DE 16 BYTES";

            try
            {
                _key = Encoding.UTF8.GetBytes(keySource).Take(32).ToArray();

                _iv = Encoding.UTF8.GetBytes(ivSource).Take(16).ToArray();

                if (_key.Length != 32 || _iv.Length != 16)
                {
                    throw new InvalidOperationException("Error de configuración de AES: La clave o el IV no tienen la longitud correcta. Verifica las cadenas keySource e ivSource.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al inicializar el servicio de encriptación.", ex);
            }
        }

        public string EncryptId(int id)
        {
            byte[] idBytes = BitConverter.GetBytes(id);
            byte[] encryptedBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    encryptedBytes = encryptor.TransformFinalBlock(idBytes, 0, idBytes.Length);
                }
            }

            string base64 = Convert.ToBase64String(encryptedBytes);

            return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public int DecryptId(string encryptedId)
        {
            string base64 = encryptedId.Replace('-', '+').Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            byte[] cipherBytes;
            try
            {
                cipherBytes = Convert.FromBase64String(base64);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Formato de ID encriptado inválido.", ex);
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] decryptedBytes;
                    try
                    {
                        decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    }
                    catch (CryptographicException ex)
                    {
                        throw new FormatException("Error de desencriptación. Posible ID corrupto o clave incorrecta.", ex);
                    }

                    if (decryptedBytes.Length != 4)
                    {
                        throw new FormatException($"El ID desencriptado no tiene el tamaño esperado (4 bytes). Tamaño real: {decryptedBytes.Length} bytes.");
                    }

                    return BitConverter.ToInt32(decryptedBytes, 0);
                }
            }
        }
    }
}
