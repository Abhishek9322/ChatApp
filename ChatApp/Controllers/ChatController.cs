using ChatApp.Models;
using ChatApp.Repository;
using ChatApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Controllers
{
   
    public class ChatController : Controller
    {

        private readonly IMessageRepository _repo;
        private readonly IAesGcmEncryptionService _encryption;
        private readonly IHubContext<ChatHub> _hub;
        public ChatController(IMessageRepository repo,IAesGcmEncryptionService encryption,IHubContext<ChatHub> hub)
        {
            _encryption = encryption;
            _repo = repo;
            _hub = hub;
        }

        [HttpGet]
        public  async Task<IActionResult> Index()
        {
            var message = await _repo.GetRecentAsync(100);

            var result = message.Select(m => new
            {
                m.Id,
                m.Sender,
                PlainText = _encryption.DecryptFromBase64(m.CipherTextBase64),
                m.CreatedAt

            });

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromForm] SendRequest req)
        {
            var cipherBase64 = _encryption.EncryptToBase64(req.Message);

            var message = new Message
            {
                Sender = req.Sender,
                CipherTextBase64 = cipherBase64
            };

            await _repo.SaveEncryptedMessageAsync(message);

            await _hub.Clients.All.SendAsync("ReceiveMessage",req.Sender,req.Message,message.CreatedAt);

            return Ok(new { save = true, id = message.Id });
        }

    }
}
