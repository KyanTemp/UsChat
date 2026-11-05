using System.ComponentModel.DataAnnotations;
using UsChat.ASP.Models.Message;
using UsChat.DTO.DTO;

namespace UsChat.ASP.Models.Chat
{
    public class ChatViewModel
    {
        public int GroupId { get; set; }  // The currently selected group
        public string GroupName { get; set; }
        public List<UserDTO> Participants { get; set; }  // The list of users in the group
        public List<MessageViewModel> Messages { get; set; }  // The actual messages in the chat
        [StringLength(250, ErrorMessage = "Bericht mag maar uit 280 characters bestaan")]
        public string NewMessageContent { get; set; }  // For sending new messages
        public bool PersonalChat { get; set; }
    }
}

