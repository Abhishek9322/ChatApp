using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Service
{
    public class AesGcmEncryptionService : IAesGcmEncryptionService
    {
        private readonly byte[] _key;
        public AesGcmEncryptionService(string base64Key)
        {
            if(string.IsNullOrEmpty(base64Key))
                throw new ArgumentNullException(nameof(base64Key));

            _key = Convert.FromBase64String(base64Key);

            if (_key.Length != 32)
                throw new ArgumentException("AES key must be 32 bytes (Base64 encoded) for AES-256-GCM.");


        }

      

        public string EncryptToBase64(string plaintext)
        {
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            var nonce = RandomNumberGenerator.GetBytes(12);
            var ciphertext = new byte[plaintextBytes.Length];
            var tag = new byte[16];

            using (var aesGcm=new AesGcm(_key))
            {
                aesGcm.Encrypt(nonce,plaintextBytes,ciphertext,tag,null);
            }

            var combined=new byte[nonce.Length + ciphertext.Length+tag.Length];
            Buffer.BlockCopy(nonce,0,combined, 0, nonce.Length);    
            Buffer.BlockCopy(ciphertext,0,combined, nonce.Length, ciphertext.Length);
            Buffer.BlockCopy(tag,0,combined, nonce.Length + ciphertext.Length, tag.Length); 

            return Convert.ToBase64String(combined);

        }
        public string DecryptFromBase64(string base64Combined)
        {
            var combined = Convert.FromBase64String(base64Combined);

            var nonce = new byte[12];
            var tag = new byte[16];
            var ciphertextlen = combined.Length - nonce.Length - tag.Length;
            var ciphertext=new byte[ciphertextlen];


            Buffer.BlockCopy(combined,0,nonce,0, nonce.Length);
            Buffer.BlockCopy(combined, nonce.Length, ciphertext, 0, ciphertextlen);
            Buffer.BlockCopy(combined, nonce.Length + ciphertextlen, tag, 0, tag.Length);

            var plaintext = new byte[ciphertextlen];

            using (var aesGcm = new AesGcm(_key))
            {
                aesGcm.Decrypt(nonce, ciphertext, tag, plaintext, null);
            }

            return Encoding.UTF8.GetString(plaintext);

        }
    }
}
