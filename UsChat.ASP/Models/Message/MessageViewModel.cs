using System.ComponentModel.DataAnnotations;

namespace UsChat.ASP.Models.Message
{
    public class MessageViewModel
    {
        [StringLength(250,ErrorMessage ="Bericht mag maar uit 280 characters bestaan")]
        public string Content { get; set; }
        public string Timestamp { get; set; }
        public bool IsFromCurrentUser { get; set; }
        public int MessageId { get; set; }
    }
}
