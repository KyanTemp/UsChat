using UsChat.DTO.DTO;

namespace UsChat.BLL.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public Message(int Id,int UserId,string Content, DateTime Timestamp)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.Content = Content;
            this.Timestamp = Timestamp;
        }
        public Message(MessageDTO DTO)
        {
            this.Id = DTO.Id;
            this.UserId = DTO.UserId;
            this.Content = DTO.Content;
            this.Timestamp = DTO.Timestamp;
        }
        public MessageDTO ToDTO()
        {
            MessageDTO DTO = new()
            {
                Id = this.Id,
                UserId = this.UserId,
                Content = this.Content,
                Timestamp = this.Timestamp
            };
            return DTO;
        }
    }
}
