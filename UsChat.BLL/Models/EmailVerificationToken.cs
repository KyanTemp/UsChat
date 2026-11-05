using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;

namespace UsChat.BLL.Models
{
    public class EmailVerificationToken
    {
        public int userId { get; set; }
        public string token { get; set; }
        public DateTime expiryDate { get; set; }
      public EmailVerificationToken(int UserId, string Token, DateTime ExpiryDate)
        {
            this.userId = UserId;
            this.token = Token;
            this.expiryDate = ExpiryDate;
        }
        public EmailVerificationToken(EmailVerificationTokenDTO DTO)
        {
            userId = DTO.userId;
            token = DTO.token;
            expiryDate = DTO.expiryDate;
        }
        public EmailVerificationTokenDTO ToDTO()
        {
            EmailVerificationTokenDTO DTO = new()
            {
                userId = this.userId,
                token = this.token,
                expiryDate = this.expiryDate
            };
            return DTO;
        }
    }
}
