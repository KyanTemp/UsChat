using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.DTO.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Content { get; set; }
        public required DateTime Timestamp { get; set; }
        public MessageDTO() { }
        public MessageDTO(int Id, int UserId,string Content, DateTime Timestamp)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.Content = Content;
            this.Timestamp = Timestamp;
        }
    }
}
