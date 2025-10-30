namespace ChatApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; } = null!;


        //for storing the secury data and send  it 
        public string CipherTextBase64 { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
