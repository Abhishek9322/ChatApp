using ChatApp.Models;
using ChatApp.Repository;
using ChatApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Controllers
{

    [Authorize]
    public class ChatController : Controller
    {

        private readonly IMessageRepository _repo;
        private readonly IAesGcmEncryptionService _encryption;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IUserRepository _userRepository;
        public ChatController(IMessageRepository repo,IAesGcmEncryptionService encryption,IHubContext<ChatHub> hub,IUserRepository userRepository)
        {
            _encryption = encryption;
            _repo = repo;
            _hub = hub;
            _userRepository = userRepository;
        }

       // [AllowAnonymous]
        [HttpGet]
        public  async Task<IActionResult> Index()
        {

            var username = User.Identity?.Name;    //problem here at time of login it not geting the proper user creadionals that login fails here 
            if (string.IsNullOrWhiteSpace(username))
                return Challenge();


            var message = await _repo.GetRecentAsync(username,100);

            var result = message
                
                .Where(m=>string.Equals(m.Sender,username, StringComparison.OrdinalIgnoreCase)
                    ||string.Equals(m.Recipient,username,StringComparison.OrdinalIgnoreCase))

               .Select(m => new
               {
                   m.Id,
                   m.Sender,
                   PlainText = _encryption.DecryptFromBase64(m.CipherTextBase64),
                   m.CreatedAt,
                   m.Recipient

               });

            return View(result);
        }
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Send([FromForm] SendRequest req)
        {
            try
            {
                var SenderName = User.Identity?.Name;
                if (string.IsNullOrWhiteSpace(SenderName))
                    return BadRequest("Unable to determine sender from token.");

                if (string.IsNullOrWhiteSpace(req.Recipient) || string.IsNullOrWhiteSpace(req.Message))
                    return BadRequest(new { save = false, error = "sender , recipient and message are required." });

                var recipientExists = await _userRepository.ExistAsync(req.Recipient);
                if (!recipientExists)
                    return BadRequest(new { save = false, error = "Recipient does not Exist." });


                var cipherBase64 = _encryption.EncryptToBase64(req.Message);

                var message = new Message
                {
                    Sender =SenderName,     
                    Recipient = req.Recipient,
                    CipherTextBase64 = cipherBase64,
                    CreatedAt = DateTime.UtcNow,
                };

                await _repo.SaveEncryptedMessageAsync(message);

                await _hub.Clients.Group(req.Recipient)
                    .SendAsync("ReceiveMessage", req.Sender, req.Message, message.CreatedAt);

                await _hub.Clients.Group(SenderName)
                    .SendAsync("ReceiveMessage", req.Sender, req.Message, message.CreatedAt);

                return Ok(new { save = true, id = message.Id });

            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
