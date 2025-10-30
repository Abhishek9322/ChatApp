namespace ChatApp.Service
{
    public interface IAesGcmEncryptionService
    {
        string EncryptToBase64(string plaintext);
        string DecryptFromBase64(string base64Combined);
    }
}
