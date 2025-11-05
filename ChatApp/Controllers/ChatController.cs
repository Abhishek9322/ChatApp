using ChatApp.Models;
using ChatApp.Repository;
using ChatApp.Service;
using Microsoft.AspNetCore.Authorization;
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

        //[Authorize]
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Send([FromForm] SendRequest req)
        {

            var SenderName = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(SenderName))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(req.Sender) || string.IsNullOrWhiteSpace(req.Recipient) || string.IsNullOrWhiteSpace(req.Message))
                return BadRequest(new { save = false, error = "sender , recipient and message are required." });

            var cipherBase64 = _encryption.EncryptToBase64(req.Message);

            var message = new Message
            {
                Sender = req.Sender,
                Recipient = req.Recipient,
                CipherTextBase64 = cipherBase64
            };

            await _repo.SaveEncryptedMessageAsync(message);

            await _hub.Clients.Group(req.Recipient).SendAsync("ReceiveMessage",req.Sender,req.Message,message.CreatedAt);

            await _hub.Clients.Group(req.Sender).SendAsync("ReceiveMessage", req.Sender, req.Message, message.CreatedAt);

            return Ok(new { save = true, id = message.Id });
        }

    }
}
